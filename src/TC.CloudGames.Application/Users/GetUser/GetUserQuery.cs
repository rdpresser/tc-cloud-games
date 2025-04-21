using TC.CloudGames.Application.Abstractions.Messaging;

namespace TC.CloudGames.Application.Users.GetUser
{
    public sealed record GetUserQuery(Guid Id) : IQuery<UserResponse>;
}