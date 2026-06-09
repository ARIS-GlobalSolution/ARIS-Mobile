using System.Globalization;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using Aris.Domain.Entities;
using Aris.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Aris.Infrastructure.Services;

public sealed class MqttTelemetryHostedService : BackgroundService
{
    private const string DefaultHost = "58d56f41a21e404084a39947b30d44ab.s1.eu.hivemq.cloud";
    private const int DefaultPort = 8883;
    private const string DefaultUser = "ARISS";
    private const string DefaultPassword = "Aris123*";

    private static readonly string[] Topics =
    [
        "aris/temperatura",
        "aris/umidade",
        "aris/nivel_agua",
    ];

    private readonly IConfiguration _configuration;
    private readonly ILogger<MqttTelemetryHostedService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly TelemetrySnapshotStore _store;

    public MqttTelemetryHostedService(
        IConfiguration configuration,
        ILogger<MqttTelemetryHostedService> logger,
        IServiceScopeFactory scopeFactory,
        TelemetrySnapshotStore store)
    {
        _configuration = configuration;
        _logger = logger;
        _scopeFactory = scopeFactory;
        _store = store;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunClientAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MQTT telemetry worker failed.");
            }

            if (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    private async Task RunClientAsync(CancellationToken stoppingToken)
    {
        var host = _configuration["Mqtt:Host"] ?? DefaultHost;
        var port = _configuration.GetValue("Mqtt:Port", DefaultPort);
        var user = _configuration["Mqtt:User"] ?? DefaultUser;
        var password = _configuration["Mqtt:Password"] ?? DefaultPassword;
        var clientId = _configuration["Mqtt:ClientId"] ?? $"aris-api-{Guid.NewGuid():N}";

        using var tcpClient = new TcpClient();
        await tcpClient.ConnectAsync(host, port, stoppingToken);

        await using var sslStream = new SslStream(tcpClient.GetStream(), false, (_, _, _, _) => true);
        await sslStream.AuthenticateAsClientAsync(host, null, System.Security.Authentication.SslProtocols.Tls12 | System.Security.Authentication.SslProtocols.Tls13, false);

        await SendConnectAsync(sslStream, clientId, user, password, stoppingToken);
        await ReadConnAckAsync(sslStream, stoppingToken);
        await SendSubscribeAsync(sslStream, Topics, stoppingToken);

        _logger.LogInformation("Connected to MQTT broker {Host}:{Port}.", host, port);

        while (!stoppingToken.IsCancellationRequested && tcpClient.Connected)
        {
            var packet = await ReadPacketAsync(sslStream, stoppingToken);

            if (packet is null)
            {
                continue;
            }

            switch (packet.Value.Type)
            {
                case 0x03:
                    await HandlePublishAsync(packet.Value.Payload, stoppingToken);
                    break;
                case 0x0D:
                    await SendPingReqAsync(sslStream, stoppingToken);
                    break;
            }
        }
    }

    private async Task HandlePublishAsync(byte[] payload, CancellationToken stoppingToken)
    {
        var topicOffset = 0;
        var topicLength = ReadUInt16BigEndian(payload, topicOffset);
        topicOffset += 2;

        var topic = Encoding.UTF8.GetString(payload, topicOffset, topicLength);
        topicOffset += topicLength;

        var message = Encoding.UTF8.GetString(payload, topicOffset, payload.Length - topicOffset).Trim();

        if (!double.TryParse(message, NumberStyles.Any, CultureInfo.InvariantCulture, out var numericValue))
        {
            _logger.LogWarning("MQTT payload from {Topic} could not be parsed: {Payload}", topic, message);
            return;
        }

        var current = _store.Current;
        var snapshot = topic switch
        {
            "aris/temperatura" => current with { Temperature = numericValue },
            "aris/umidade" => current with { Humidity = numericValue },
            "aris/nivel_agua" => current with { WaterLevel = numericValue },
            _ => current
        };

        snapshot = BuildSnapshot(snapshot);
        _store.Update(snapshot);

        await PersistTelemetryAsync(topic, numericValue, stoppingToken);
    }

    private static TelemetrySnapshot BuildSnapshot(TelemetrySnapshot snapshot)
    {
        var issues = new List<string>();

        if (snapshot.Temperature is { } temperature && (temperature < 20 || temperature > 30))
        {
            issues.Add("Temperatura fora da faixa");
        }

        if (snapshot.Humidity is { } humidity && (humidity < 40 || humidity > 80))
        {
            issues.Add("Umidade fora da faixa");
        }

        if (snapshot.WaterLevel is { } waterLevel && waterLevel > 20)
        {
            issues.Add("Nível de água acima do limite");
        }

        var alert = issues.Count > 0;
        var status = alert ? "Atenção" : "Estável";
        var message = alert ? string.Join(" • ", issues) : "Tudo estável na base.";

        return snapshot with
        {
            CapturedAt = DateTime.UtcNow,
            Alert = alert,
            Status = status,
            Message = message,
        };
    }

    private async Task PersistTelemetryAsync(string topic, double value, CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ArisDbContext>();

            var sensorId = topic switch
            {
                "aris/temperatura" => 1,
                "aris/umidade" => 2,
                "aris/nivel_agua" => 3,
                _ => 1,
            };

            var sensorExists = await dbContext.Set<Sensor>()
                .Where(x => x.Id == sensorId)
                .CountAsync(stoppingToken) > 0;
            if (!sensorExists)
            {
                return;
            }

            var entity = new Telemetria((decimal)value, sensorId);

            dbContext.Set<Telemetria>().Add(entity);
            await dbContext.SaveChangesAsync(stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not persist telemetry to Oracle.");
        }
    }

    private static void TrySetProperty(object target, string propertyName, object? value)
    {
        var property = target.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

        if (property is null || !property.CanWrite || value is null)
        {
            return;
        }

        var targetType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

        try
        {
            var convertedValue = targetType.IsEnum
                ? Enum.Parse(targetType, value.ToString() ?? string.Empty, ignoreCase: true)
                : Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);

            property.SetValue(target, convertedValue);
        }
        catch
        {
            // Best-effort mapping for legacy Oracle schemas.
        }
    }

    private static async Task SendConnectAsync(Stream stream, string clientId, string username, string password, CancellationToken stoppingToken)
    {
        using var body = new MemoryStream();
        WriteMqttString(body, "MQTT");
        body.WriteByte(0x04);
        body.WriteByte(0xC2);
        WriteUInt16BigEndian(body, 60);
        WriteMqttString(body, clientId);
        WriteMqttString(body, username);
        WriteMqttString(body, password);

        await WritePacketAsync(stream, 0x10, body.ToArray(), stoppingToken);
    }

    private static async Task SendSubscribeAsync(Stream stream, IEnumerable<string> topics, CancellationToken stoppingToken)
    {
        using var body = new MemoryStream();
        WriteUInt16BigEndian(body, 1);

        foreach (var topic in topics)
        {
            WriteMqttString(body, topic);
            body.WriteByte(0x00);
        }

        await WritePacketAsync(stream, 0x82, body.ToArray(), stoppingToken);
    }

    private static async Task SendPingReqAsync(Stream stream, CancellationToken stoppingToken)
    {
        await WritePacketAsync(stream, 0xC0, Array.Empty<byte>(), stoppingToken);
    }

    private static async Task ReadConnAckAsync(Stream stream, CancellationToken stoppingToken)
    {
        var packet = await ReadPacketAsync(stream, stoppingToken);
        if (packet is null || packet.Value.Type != 0x02)
        {
            throw new InvalidOperationException("Invalid CONNACK packet.");
        }
    }

    private static async Task<(byte Type, byte[] Payload)?> ReadPacketAsync(Stream stream, CancellationToken stoppingToken)
    {
        var header = await ReadExactAsync(stream, 1, stoppingToken);
        if (header.Length == 0)
        {
            return null;
        }

        var remainingLength = await ReadRemainingLengthAsync(stream, stoppingToken);
        var payload = remainingLength == 0 ? Array.Empty<byte>() : await ReadExactAsync(stream, remainingLength, stoppingToken);

        return ((byte)(header[0] >> 4), payload);
    }

    private static async Task<int> ReadRemainingLengthAsync(Stream stream, CancellationToken stoppingToken)
    {
        var multiplier = 1;
        var value = 0;
        byte encodedByte;

        do
        {
            var buffer = await ReadExactAsync(stream, 1, stoppingToken);
            encodedByte = buffer[0];
            value += (encodedByte & 127) * multiplier;
            multiplier *= 128;
        }
        while ((encodedByte & 128) != 0);

        return value;
    }

    private static async Task<byte[]> ReadExactAsync(Stream stream, int count, CancellationToken stoppingToken)
    {
        var buffer = new byte[count];
        var offset = 0;

        while (offset < count)
        {
            var read = await stream.ReadAsync(buffer.AsMemory(offset, count - offset), stoppingToken);
            if (read == 0)
            {
                throw new IOException("MQTT connection closed.");
            }

            offset += read;
        }

        return buffer;
    }

    private static async Task WritePacketAsync(Stream stream, byte header, byte[] payload, CancellationToken stoppingToken)
    {
        using var packet = new MemoryStream();
        packet.WriteByte(header);
        WriteRemainingLength(packet, payload.Length);
        packet.Write(payload, 0, payload.Length);

        var buffer = packet.ToArray();
        await stream.WriteAsync(buffer.AsMemory(0, buffer.Length), stoppingToken);
        await stream.FlushAsync(stoppingToken);
    }

    private static void WriteRemainingLength(Stream stream, int length)
    {
        do
        {
            var encodedByte = length % 128;
            length /= 128;

            if (length > 0)
            {
                encodedByte |= 128;
            }

            stream.WriteByte((byte)encodedByte);
        }
        while (length > 0);
    }

    private static void WriteMqttString(Stream stream, string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        WriteUInt16BigEndian(stream, bytes.Length);
        stream.Write(bytes, 0, bytes.Length);
    }

    private static void WriteUInt16BigEndian(Stream stream, int value)
    {
        stream.WriteByte((byte)((value >> 8) & 0xFF));
        stream.WriteByte((byte)(value & 0xFF));
    }

    private static int ReadUInt16BigEndian(byte[] buffer, int offset)
    {
        return (buffer[offset] << 8) | buffer[offset + 1];
    }
}
