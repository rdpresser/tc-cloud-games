using Microsoft.EntityFrameworkCore;
using TC.CloudGames.Domain.Abstractions;
using TC.CloudGames.Infra.CrossCutting.Commons.Clock;

namespace TC.CloudGames.Infra.Data.Repositories.EfCore;

public abstract class EfRepository<TEntity> : IEfRepository<TEntity>
    where TEntity : Entity
{
    protected readonly ApplicationDbContext DbContext;
    protected readonly DbSet<TEntity> DbSet;
    private readonly IDateTimeProvider _dateTimeProvider;

    protected EfRepository(ApplicationDbContext dbContext, IDateTimeProvider dateTimeProvider)
    {
        DbContext = dbContext;
        _dateTimeProvider = dateTimeProvider;
        DbSet = dbContext.Set<TEntity>();
    }

    public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken).ConfigureAwait(false);
    }

    public void Add(TEntity entity)
    {
        entity.CreatedOnUtc = _dateTimeProvider.UtcNow;
        DbSet.Add(entity);
    }

    public void AddRange(IEnumerable<TEntity> entities)
    {
        entities.ToList().ForEach(e => e.CreatedOnUtc = _dateTimeProvider.UtcNow);
        DbSet.AddRange(entities);
    }
}