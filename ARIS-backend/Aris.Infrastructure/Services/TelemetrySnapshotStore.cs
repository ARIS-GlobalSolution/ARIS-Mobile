namespace Aris.Infrastructure.Services;

public sealed record TelemetrySnapshot(
    DateTime CapturedAt,
    double? Temperature,
    double? Humidity,
    double? WaterLevel,
    bool Alert,
    string Status,
    string Message);

public sealed class TelemetrySnapshotStore
{
    private readonly object _lock = new();
    private TelemetrySnapshot _current = new(
        DateTime.UtcNow,
        null,
        null,
        null,
        false,
        "Sem leitura",
        "Aguardando leitura do MQTT.");

    public TelemetrySnapshot Current
    {
        get
        {
            lock (_lock)
            {
                return _current;
            }
        }
    }

    public void Update(TelemetrySnapshot snapshot)
    {
        lock (_lock)
        {
            _current = snapshot;
        }
    }
}
