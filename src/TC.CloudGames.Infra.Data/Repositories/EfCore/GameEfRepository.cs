using TC.CloudGames.Domain.Game;
using TC.CloudGames.Domain.Game.Abstractions;
using TC.CloudGames.Infra.CrossCutting.Commons.Clock;

namespace TC.CloudGames.Infra.Data.Repositories.EfCore
{
    public sealed class GameEfRepository : EfRepository<Game>, IGameEfRepository
    {
        public GameEfRepository(ApplicationDbContext dbContext, IDateTimeProvider dateTimeProvider)
            : base(dbContext, dateTimeProvider)
        {

        }
    }
}
