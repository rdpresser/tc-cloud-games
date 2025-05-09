namespace TC.CloudGames.Infra.CrossCutting.Commons.Middleware
{
    public interface ICorrelationIdGenerator
    {
        string Get();
        void Set(string correlationId);
    }
}
