using TC.CloudGames.Domain.Game;
using TC.CloudGames.Domain.User;

namespace TC.CloudGames.Application.Abstractions.Data
{
    public interface IUnitOfWork
    {
        public DbSet<User> Users { get; }
        public DbSet<Game> Games { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
