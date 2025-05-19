namespace TC.CloudGames.Application.Users.CreateUser
{
    public sealed record CreateUserCommand(
        string FirstName,
        string LastName,
        string Email,
        string Password,
        string Role) : Abstractions.Messaging.ICommand<CreateUserResponse>;
}
