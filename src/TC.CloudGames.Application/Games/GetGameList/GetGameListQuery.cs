namespace TC.CloudGames.Application.Games.GetGameList
{
    public sealed record GetGameListQuery(
        int PageNumber = 1,
        int PageSize = 10,
        string SortBy = "id",
        string SortDirection = "asc",
        string Filter = ""
    ) : IQuery<IReadOnlyList<GameListResponse>>;
}
