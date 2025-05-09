namespace TC.CloudGames.Domain.Abstractions;

public interface IEfRepository<TEntity> where TEntity : Entity
{
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    void Add(TEntity entity);
}