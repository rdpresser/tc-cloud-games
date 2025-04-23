namespace TC.CloudGames.Application.Users.Login
{
    public sealed record LoginUserResponse(
        string JwtToken,
        string Email);
}
