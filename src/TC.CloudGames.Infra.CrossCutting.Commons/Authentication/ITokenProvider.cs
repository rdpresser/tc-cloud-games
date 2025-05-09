namespace TC.CloudGames.Infra.CrossCutting.Commons.Authentication
{
    public interface ITokenProvider
    {
        string Create(UserTokenProvider user);
    }
}
