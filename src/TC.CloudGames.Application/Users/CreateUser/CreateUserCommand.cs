using TC.CloudGames.Application.Abstractions.Messaging;

namespace TC.CloudGames.Application.Users.CreateUser
{
    public sealed record CreateUserCommand(
        string FirstName,
        string LastName,
        string Email,
        string Password,
        string Role) : ICommand<CreateUserResponse>;
}
