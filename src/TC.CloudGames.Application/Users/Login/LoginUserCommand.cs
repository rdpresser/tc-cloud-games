using TC.CloudGames.Application.Abstractions.Messaging;

namespace TC.CloudGames.Application.Users.Login
{
    public sealed record LoginUserCommand(
        string Email,
        string Password) : ICommand<LoginUserResponse>;
}
