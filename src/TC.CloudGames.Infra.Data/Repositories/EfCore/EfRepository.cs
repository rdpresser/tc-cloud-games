using Microsoft.EntityFrameworkCore;
using TC.CloudGames.Domain.Abstractions;

namespace TC.CloudGames.Infra.Data.Repositories.EfCore;

public abstract class EfRepository<TEntity>
    where TEntity : Entity
{
    protected readonly ApplicationDbContext DbContext;

    protected EfRepository(ApplicationDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<TEntity>()
            .FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken).ConfigureAwait(false);
    }

    public void Add(TEntity entity)
    {
        DbContext.Add(entity);
    }
}