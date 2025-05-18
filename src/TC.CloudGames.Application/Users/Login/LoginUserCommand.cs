namespace TC.CloudGames.Application.Users.Login
{
    public sealed record LoginUserCommand(
        string Email,
        string Password) : Abstractions.Messaging.ICommand<LoginUserResponse>;
}
