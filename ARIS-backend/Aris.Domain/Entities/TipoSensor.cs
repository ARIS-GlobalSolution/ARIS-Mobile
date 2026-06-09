using Aris.Domain.Commons;

namespace Aris.Domain.Entities;

public class TipoSensor : BaseEntity
{
    public string Nome { get; private set; } = string.Empty;
    public string? Unidade { get; private set; }

    public List<Sensor> Sensores { get; private set; } = new();

    private TipoSensor() { }

    public TipoSensor(string nome, string? unidade)
    {
        Update(nome, unidade);
    }

    public void Update(string nome, string? unidade)
    {
        Nome = string.IsNullOrWhiteSpace(nome) ? throw new ArgumentException("Nome invalido.") : nome.Trim();
        Unidade = string.IsNullOrWhiteSpace(unidade) ? null : unidade.Trim();
    }
}
