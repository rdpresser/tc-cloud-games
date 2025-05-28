using TC.CloudGames.Domain.GameAggregate;
using TC.CloudGames.Domain.UserAggregate;

namespace TC.CloudGames.Application.Abstractions.Data
{
    public interface IUnitOfWork
    {
        public DbSet<User> Users { get; }
        public DbSet<Game> Games { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
