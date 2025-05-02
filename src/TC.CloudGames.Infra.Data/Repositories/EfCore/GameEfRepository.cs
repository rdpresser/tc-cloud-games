using TC.CloudGames.Domain.Game;

namespace TC.CloudGames.Infra.Data.Repositories.EfCore
{
    public sealed class GameEfRepository : EfRepository<Game>, IGameEfRepository
    {
        public GameEfRepository(ApplicationDbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
