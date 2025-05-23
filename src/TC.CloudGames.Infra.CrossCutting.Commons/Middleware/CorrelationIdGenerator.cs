namespace TC.CloudGames.Infra.CrossCutting.Commons.Middleware
{
    public class CorrelationIdGenerator : ICorrelationIdGenerator
    {
        public string CorrelationId { get; private set; } = string.Empty;

        public void SetCorrelationId(string correlationId)
        {
            CorrelationId = correlationId;
        }
    }
}
