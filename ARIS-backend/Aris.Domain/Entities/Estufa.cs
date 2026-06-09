using Aris.Domain.Commons;

namespace Aris.Domain.Entities;

public class Estufa : BaseEntity
{
    public string Nome { get; private set; } = string.Empty;
    public string? Localizacao { get; private set; }
    public int UsuarioId { get; private set; }
    public Usuario Usuario { get; private set; } = null!;

    public List<Sensor> Sensores { get; private set; } = new();
    public List<Cultura> Culturas { get; private set; } = new();
    public List<Irrigacao> Irrigacoes { get; private set; } = new();
    public List<Alerta> Alertas { get; private set; } = new();

    private Estufa() { }

    public Estufa(string nome, string? localizacao, int usuarioId)
    {
        Update(nome, localizacao, usuarioId);
    }

    public void Update(string nome, string? localizacao, int usuarioId)
    {
        Nome = string.IsNullOrWhiteSpace(nome) ? throw new ArgumentException("Nome invalido.") : nome.Trim();
        Localizacao = string.IsNullOrWhiteSpace(localizacao) ? null : localizacao.Trim();
        UsuarioId = usuarioId > 0 ? usuarioId : throw new ArgumentException("Usuario invalido.");
    }
}
