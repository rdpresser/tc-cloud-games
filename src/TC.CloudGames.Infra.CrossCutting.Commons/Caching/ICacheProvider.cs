namespace TC.CloudGames.Infra.CrossCutting.Commons.Caching
{
    public interface ICacheProvider
    {
        string InstanceName { get; }
        string ConnectionString { get; }
    }
}
