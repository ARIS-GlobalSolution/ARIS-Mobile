using Aris.Domain.Commons;

namespace Aris.Domain.Entities;

public class Irrigacao : BaseEntity
{
    public DateTime DataHora { get; private set; } = DateTime.UtcNow;
    public string Status { get; private set; } = "AGUARDANDO";
    public int EstufaId { get; private set; }
    public Estufa Estufa { get; private set; } = null!;

    private Irrigacao() { }

    public Irrigacao(string status, int estufaId)
    {
        Update(status, estufaId);
    }

    public void Update(string status, int estufaId)
    {
        Status = string.IsNullOrWhiteSpace(status) ? throw new ArgumentException("Status invalido.") : status.Trim().ToUpperInvariant();
        EstufaId = estufaId > 0 ? estufaId : throw new ArgumentException("Estufa invalida.");
        DataHora = DateTime.UtcNow;
    }
}
