namespace TC.CloudGames.Infra.Data.Configurations.Data
{
    public interface IDatabaseConnectionProvider
    {
        string ConnectionString { get; }
    }
}
