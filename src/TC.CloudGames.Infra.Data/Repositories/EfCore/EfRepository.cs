using Microsoft.EntityFrameworkCore;
using TC.CloudGames.Domain.Abstractions;
using TC.CloudGames.Infra.CrossCutting.Commons.Clock;

namespace TC.CloudGames.Infra.Data.Repositories.EfCore;

public abstract class EfRepository<TEntity> : IEfRepository<TEntity>, IDisposable, IAsyncDisposable
    where TEntity : Entity
{
    protected readonly ApplicationDbContext DbContext;
    protected readonly DbSet<TEntity> DbSet;

    private readonly IDateTimeProvider _dateTimeProvider;
    private bool _disposed;

    protected EfRepository(ApplicationDbContext dbContext, IDateTimeProvider dateTimeProvider)
    {
        DbContext = dbContext;
        _dateTimeProvider = dateTimeProvider;
        DbSet = dbContext.Set<TEntity>();
    }

    public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FindAsync(id, cancellationToken).ConfigureAwait(false);
    }

    public void Add(TEntity entity)
    {
        entity.SetCreatedOnUtc(_dateTimeProvider.UtcNow);
        DbSet.Add(entity);
    }

    public void AddRange(IEnumerable<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            entity.SetCreatedOnUtc(_dateTimeProvider.UtcNow);
        }

        DbSet.AddRange(entities);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken)
            .ConfigureAwait(false);

        if (entity != null)
        {
            Delete(entity);
        }
    }

    public void Delete(TEntity entity)
    {
        DbSet.Remove(entity);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                DbContext.Dispose();
            }

            _disposed = true;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore();

        // Suppress finalization to prevent the finalizer from running
        GC.SuppressFinalize(this);
    }

    private async ValueTask DisposeAsyncCore()
    {
        if (!_disposed)
        {
            if (DbContext != null)
            {
                await DbContext.DisposeAsync().ConfigureAwait(false);
            }

            _disposed = true;
        }
    }
}