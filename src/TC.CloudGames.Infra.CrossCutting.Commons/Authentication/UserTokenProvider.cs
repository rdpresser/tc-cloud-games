namespace TC.CloudGames.Infra.CrossCutting.Commons.Authentication
{
    public sealed record UserTokenProvider(Guid Id, string FirstName, string LastName, string Email, string Role);
}
