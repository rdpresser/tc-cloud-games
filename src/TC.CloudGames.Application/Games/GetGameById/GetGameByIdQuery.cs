namespace TC.CloudGames.Application.Games.GetGameById
{
    public sealed record GetGameByIdQuery(Guid Id) : IQuery<GameByIdResponse>;
}
