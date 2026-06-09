using Aris.Domain.Commons;

namespace Aris.Domain.Entities;

public class ParametroCultura : BaseEntity
{
    public int CulturaId { get; private set; }
    public decimal? TempMin { get; private set; }
    public decimal? TempMax { get; private set; }
    public decimal? UmidadeMin { get; private set; }
    public decimal? UmidadeMax { get; private set; }

    public Cultura Cultura { get; private set; } = null!;

    private ParametroCultura() { }

    public ParametroCultura(int culturaId, decimal? tempMin, decimal? tempMax, decimal? umidadeMin, decimal? umidadeMax)
    {
        Update(culturaId, tempMin, tempMax, umidadeMin, umidadeMax);
    }

    public void Update(int culturaId, decimal? tempMin, decimal? tempMax, decimal? umidadeMin, decimal? umidadeMax)
    {
        CulturaId = culturaId > 0 ? culturaId : throw new ArgumentException("Cultura invalida.");
        TempMin = tempMin;
        TempMax = tempMax;
        UmidadeMin = umidadeMin;
        UmidadeMax = umidadeMax;
    }
}
