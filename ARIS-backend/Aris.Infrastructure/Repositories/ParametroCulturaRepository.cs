using Aris.Application.Interfaces.Repositories;
using Aris.Domain.Entities;
using Aris.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Aris.Infrastructure.Repositories;

public class ParametroCulturaRepository : Repository<ParametroCultura>, IParametroCulturaRepository
{
    private readonly ArisDbContext _context;

    public ParametroCulturaRepository(ArisDbContext context) : base(context)
    {
        _context = context;
    }

    public Task<ParametroCultura?> GetByCulturaIdAsync(int culturaId, CancellationToken cancellationToken = default) =>
        _context.ParametrosCultura.FirstOrDefaultAsync(x => x.CulturaId == culturaId, cancellationToken);
}
