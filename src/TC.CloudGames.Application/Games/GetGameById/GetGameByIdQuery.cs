using TC.CloudGames.Application.Abstractions.Messaging;
using TC.CloudGames.Application.Games.GetGameById;

namespace TC.CloudGames.Application.Games.GetGameById
{
    public sealed record GetGameByIdQuery(Guid Id) : IQuery<GameByIdResponse>;
}
