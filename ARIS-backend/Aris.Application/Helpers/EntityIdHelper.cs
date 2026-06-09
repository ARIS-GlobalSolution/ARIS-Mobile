using System.Reflection;
using Aris.Application.Interfaces.Repositories;
using Aris.Domain.Commons;
using Microsoft.EntityFrameworkCore;

namespace Aris.Application.Helpers;

public static class EntityIdHelper
{
    public static async Task<int> GetNextIdAsync<TEntity>(IRepository<TEntity> repository, CancellationToken cancellationToken = default)
        where TEntity : BaseEntity
    {
        var ids = await repository.Query()
            .Select(entity => entity.Id)
            .ToListAsync(cancellationToken);

        var currentMaxId = ids.Count == 0 ? 0 : ids.Max();

        return currentMaxId + 1;
    }

    public static void SetId<TEntity>(TEntity entity, int id)
        where TEntity : BaseEntity
    {
        var property = typeof(BaseEntity).GetProperty(
            nameof(BaseEntity.Id),
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        property?.SetValue(entity, id);
    }
}
