﻿using TC.CloudGames.Domain.Aggregates.Game;
using TC.CloudGames.Domain.Aggregates.Game.Abstractions;
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
