using TC.CloudGames.Application.Abstractions.Messaging;

namespace TC.CloudGames.Application.Games.GetGame
{
    public sealed record GetGameQuery(Guid Id) : IQuery<GameResponse>;
}
