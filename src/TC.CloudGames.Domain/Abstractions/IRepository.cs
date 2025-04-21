namespace TC.CloudGames.Domain.Abstractions;

public interface IRepository<TEntity> where TEntity : Entity
{
    public Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    void Add(TEntity entity);
}