using TC.CloudGames.Application.Abstractions.Messaging;

namespace TC.CloudGames.Application.Games.GetGame
{
    public sealed record GetGameByIdQuery(Guid Id) : IQuery<GameResponse>;
}
