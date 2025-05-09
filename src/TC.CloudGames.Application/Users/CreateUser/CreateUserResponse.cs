namespace TC.CloudGames.Application.Users.CreateUser
{
    public sealed record CreateUserResponse(
        Guid Id,
        string FirstName,
        string LastName,
        string Email,
        string Role);
}
