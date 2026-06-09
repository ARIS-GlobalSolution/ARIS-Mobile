using Aris.Domain.Entities;

namespace Aris.Application.Interfaces.Repositories;

public interface IParametroCulturaRepository : IRepository<ParametroCultura>
{
    Task<ParametroCultura?> GetByCulturaIdAsync(int culturaId, CancellationToken cancellationToken = default);
}
