namespace TC.CloudGames.Application.Users.GetUserList
{
    public sealed record GetUserListQuery(
        int PageNumber = 1,
        int PageSize = 10,
        string SortBy = "id",
        string SortDirection = "asc",
        string Filter = ""
    ) : IQuery<IReadOnlyList<UserListResponse>>;
}
