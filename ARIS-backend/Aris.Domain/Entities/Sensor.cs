using Aris.Domain.Commons;

namespace Aris.Domain.Entities;

public class Sensor : BaseEntity
{
    public int TipoSensorId { get; private set; }
    public int EstufaId { get; private set; }

    public TipoSensor TipoSensor { get; private set; } = null!;
    public Estufa Estufa { get; private set; } = null!;

    public List<Telemetria> Telemetrias { get; private set; } = new();

    private Sensor() { }

    public Sensor(int tipoSensorId, int estufaId)
    {
        Update(tipoSensorId, estufaId);
    }

    public void Update(int tipoSensorId, int estufaId)
    {
        TipoSensorId = tipoSensorId > 0 ? tipoSensorId : throw new ArgumentException("Tipo de sensor invalido.");
        EstufaId = estufaId > 0 ? estufaId : throw new ArgumentException("Estufa invalida.");
    }
}
