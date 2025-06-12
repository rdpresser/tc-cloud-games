using TC.CloudGames.Domain.Aggregates.Game;
using TC.CloudGames.Domain.Aggregates.User;

namespace TC.CloudGames.Application.Abstractions.Data
{
    public interface IUnitOfWork
    {
        public DbSet<User> Users { get; }
        public DbSet<Game> Games { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
