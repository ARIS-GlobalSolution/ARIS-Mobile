using Aris.Domain.Commons;

namespace Aris.Domain.Entities;

public class Alerta : BaseEntity
{
    public string Mensagem { get; private set; } = string.Empty;
    public string NivelRisco { get; private set; } = string.Empty;
    public DateTime DataHora { get; private set; } = DateTime.UtcNow;
    public int EstufaId { get; private set; }
    public Estufa Estufa { get; private set; } = null!;

    private Alerta() { }

    public Alerta(string mensagem, string nivelRisco, int estufaId)
    {
        Update(mensagem, nivelRisco, estufaId);
    }

    public void Update(string mensagem, string nivelRisco, int estufaId)
    {
        Mensagem = string.IsNullOrWhiteSpace(mensagem) ? throw new ArgumentException("Mensagem invalida.") : mensagem.Trim();
        NivelRisco = string.IsNullOrWhiteSpace(nivelRisco) ? throw new ArgumentException("Nivel de risco invalido.") : nivelRisco.Trim().ToUpperInvariant();
        EstufaId = estufaId > 0 ? estufaId : throw new ArgumentException("Estufa invalida.");
        DataHora = DateTime.UtcNow;
    }
}
