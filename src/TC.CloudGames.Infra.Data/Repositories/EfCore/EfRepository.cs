using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using TC.CloudGames.Domain.Abstractions;
using TC.CloudGames.Infra.CrossCutting.Commons.Clock;

namespace TC.CloudGames.Infra.Data.Repositories.EfCore;

[ExcludeFromCodeCoverage]
public abstract class EfRepository<TEntity> : IEfRepository<TEntity>, IDisposable, IAsyncDisposable
    where TEntity : Entity
{
    private bool _disposed;

    private readonly IDateTimeProvider _dateTimeProvider;
    protected ApplicationDbContext DbContext { get; }
    protected DbSet<TEntity> DbSet { get; }

    protected EfRepository(ApplicationDbContext dbContext, IDateTimeProvider dateTimeProvider)
    {
        DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        DbSet = dbContext.Set<TEntity>();
    }

    public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken).ConfigureAwait(false);
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

    public async Task BulkInsertAsync(IEnumerable<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            entity.SetCreatedOnUtc(_dateTimeProvider.UtcNow);
        }

        await DbSet.BulkInsertAsync(entities).ConfigureAwait(false);
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
        await DisposeAsyncCore().ConfigureAwait(false);

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