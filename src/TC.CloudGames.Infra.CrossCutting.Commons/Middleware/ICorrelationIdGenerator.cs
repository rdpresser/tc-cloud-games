namespace TC.CloudGames.Infra.CrossCutting.Commons.Middleware
{
    public interface ICorrelationIdGenerator
    {
        string CorrelationId { get; }
        void SetCorrelationId(string correlationId);
    }
}
