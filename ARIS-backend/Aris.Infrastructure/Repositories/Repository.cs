using Aris.Application.Interfaces.Repositories;
using Aris.Domain.Commons;
using Aris.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Aris.Infrastructure.Repositories;

public class Repository<TEntity>(ArisDbContext context) : IRepository<TEntity> where TEntity : BaseEntity
{
    private readonly DbSet<TEntity> _set = context.Set<TEntity>();

    public IQueryable<TEntity> Query() => _set.AsQueryable();

    public Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default) =>
        _set.AsNoTracking().ToListAsync(cancellationToken);

    public Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _set.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task AddAsync(TEntity entity, CancellationToken cancellationToken = default) =>
        _set.AddAsync(entity, cancellationToken).AsTask();

    public void Update(TEntity entity) => _set.Update(entity);

    public void Remove(TEntity entity) => _set.Remove(entity);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
