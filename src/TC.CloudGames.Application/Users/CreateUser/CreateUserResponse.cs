namespace TC.CloudGames.Application.Users.CreateUser
{
    public record CreateUserResponse(Guid Id, string FirstName, string LastName, string Email, string Role);
}
