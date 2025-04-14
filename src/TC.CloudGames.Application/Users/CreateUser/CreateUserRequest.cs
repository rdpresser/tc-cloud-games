namespace TC.CloudGames.Application.Users.CreateUser
{
    public record CreateUserRequest(string Name, string Email, string Password, string Role);
}
