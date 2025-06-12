using TC.CloudGames.Domain.Entities;

namespace TC.CloudGames.Domain.Abstractions;

public interface IEfRepository<TEntity> where TEntity : Entity
{
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    void Add(TEntity entity);

    void AddRange(IEnumerable<TEntity> entities);

    Task BulkInsertAsync(IEnumerable<TEntity> entities);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}