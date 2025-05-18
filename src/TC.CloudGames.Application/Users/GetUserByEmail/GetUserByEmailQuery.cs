namespace TC.CloudGames.Application.Users.GetUserByEmail
{
    public sealed record GetUserByEmailQuery(string Email) : IQuery<UserByEmailResponse>;
}