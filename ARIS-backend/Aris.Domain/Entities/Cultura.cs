using Aris.Domain.Commons;

namespace Aris.Domain.Entities;

public class Cultura : BaseEntity
{
    public string Nome { get; private set; } = string.Empty;
    public int EstufaId { get; private set; }
    public Estufa Estufa { get; private set; } = null!;
    public ParametroCultura? ParametroCultura { get; private set; }

    private Cultura() { }

    public Cultura(string nome, int estufaId)
    {
        Update(nome, estufaId);
    }

    public void Update(string nome, int estufaId)
    {
        Nome = string.IsNullOrWhiteSpace(nome) ? throw new ArgumentException("Nome invalido.") : nome.Trim();
        EstufaId = estufaId > 0 ? estufaId : throw new ArgumentException("Estufa invalida.");
    }
}
