namespace TC.CloudGames.Application.Users.GetUserById
{
    public sealed record GetUserByIdQuery(Guid Id) : IQuery<UserByIdResponse>;
}
