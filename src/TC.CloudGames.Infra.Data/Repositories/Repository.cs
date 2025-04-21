using Microsoft.EntityFrameworkCore;
using TC.CloudGames.Domain.Abstractions;

namespace TC.CloudGames.Infra.Data.Repositories
{
    public abstract class Repository<TEntity>
            where TEntity : Entity
    {
        protected readonly ApplicationDbContext DbContext;

        protected Repository(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await DbContext
                .Set<TEntity>()
                .FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken);
        }

        public void Add(TEntity entity)
        {
            DbContext.Add(entity);
        }
    }
}
