using Aris.Domain.Commons;

namespace Aris.Domain.Entities;

public class LogAcao : BaseEntity
{
    public string Acao { get; private set; } = string.Empty;
    public string Descricao { get; private set; } = string.Empty;
    public DateTime DataHora { get; private set; } = DateTime.UtcNow;

    private LogAcao() { }

    public LogAcao(string acao, string descricao)
    {
        Acao = string.IsNullOrWhiteSpace(acao) ? throw new ArgumentException("Acao invalida.") : acao.Trim();
        Descricao = string.IsNullOrWhiteSpace(descricao) ? throw new ArgumentException("Descricao invalida.") : descricao.Trim();
        DataHora = DateTime.UtcNow;
    }
}
