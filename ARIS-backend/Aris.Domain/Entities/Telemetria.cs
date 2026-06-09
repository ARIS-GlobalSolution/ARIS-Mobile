using Aris.Domain.Commons;

namespace Aris.Domain.Entities;

public class Telemetria : BaseEntity
{
    public decimal Valor { get; private set; }
    public DateTime DataHora { get; private set; } = DateTime.UtcNow;
    public int SensorId { get; private set; }
    public Sensor Sensor { get; private set; } = null!;

    private Telemetria() { }

    public Telemetria(decimal valor, int sensorId)
    {
        Update(valor, sensorId);
    }

    public void Update(decimal valor, int sensorId)
    {
        Valor = valor;
        SensorId = sensorId > 0 ? sensorId : throw new ArgumentException("Sensor invalido.");
        DataHora = DateTime.UtcNow;
    }
}
