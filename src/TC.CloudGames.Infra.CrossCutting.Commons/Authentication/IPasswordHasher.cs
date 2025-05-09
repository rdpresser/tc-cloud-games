namespace TC.CloudGames.Infra.CrossCutting.Commons.Authentication
{
    public interface IPasswordHasher
    {
        string Hash(string password);

        bool Verify(string password, string passwordHash);
    }
}
