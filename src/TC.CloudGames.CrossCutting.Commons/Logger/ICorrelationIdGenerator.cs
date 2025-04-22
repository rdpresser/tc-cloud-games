namespace TC.CloudGames.CrossCutting.Commons.Logger
{
    public interface ICorrelationIdGenerator
    {
        string Get();
        void Set(string correlationId);
    }
}
