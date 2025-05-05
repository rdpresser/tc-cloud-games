namespace TC.CloudGames.CrossCutting.Commons.Middleware
{
    public interface ICorrelationIdGenerator
    {
        string Get();
        void Set(string correlationId);
    }
}
