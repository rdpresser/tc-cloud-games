using Microsoft.Extensions.Logging;

namespace TC.CloudGames.CrossCutting.Commons.Logger
{
    public class BaseLogger<T>
        where T : class
    {
        protected readonly ILogger<T> _logger;
        protected readonly ICorrelationIdGenerator _correlationId;

        public BaseLogger(ILogger<T> logger, ICorrelationIdGenerator correlationId)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _correlationId = correlationId ?? throw new ArgumentNullException(nameof(correlationId));
        }

        public virtual void LogInformation(string message)
        {
            _logger.LogInformation("[CorrelationId: {CorrelationId}] | {Message}", _correlationId.Get(), message);
        }

        public virtual void LogError(string message)
        {
            _logger.LogError("[CorrelationId: {CorrelationId}] | {Message}", _correlationId.Get(), message);
        }

        public virtual void LogWarning(string message)
        {
            _logger.LogWarning("[CorrelationId: {CorrelationId}] | {Message}", _correlationId.Get(), message);
        }
    }
}
