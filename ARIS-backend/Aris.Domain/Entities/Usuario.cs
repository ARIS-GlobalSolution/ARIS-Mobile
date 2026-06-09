using Aris.Domain.Commons;

namespace Aris.Domain.Entities;

public class Usuario : BaseEntity
{
    public string Nome { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string SenhaHash { get; private set; } = string.Empty;
    public DateTime DataCadastro { get; private set; } = DateTime.UtcNow;

    public List<Estufa> Estufas { get; private set; } = new();

    private Usuario() { }

    public Usuario(string nome, string email, string senhaHash)
    {
        Update(nome, email, senhaHash);
        DataCadastro = DateTime.UtcNow;
    }

    public void Update(string nome, string email, string senhaHash)
    {
        Nome = string.IsNullOrWhiteSpace(nome) ? throw new ArgumentException("Nome invalido.") : nome.Trim();
        Email = string.IsNullOrWhiteSpace(email) ? throw new ArgumentException("Email invalido.") : email.Trim().ToLowerInvariant();
        SenhaHash = string.IsNullOrWhiteSpace(senhaHash) ? throw new ArgumentException("Senha invalida.") : senhaHash.Trim();
    }
}
