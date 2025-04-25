using TC.CloudGames.Domain.Game;

namespace TC.CloudGames.Infra.Data.Repositories
{
    public sealed class GameRepository : Repository<Game>, IGameRepository
    {
        public GameRepository(ApplicationDbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
