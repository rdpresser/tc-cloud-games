using Serilog.Context;
using System.Diagnostics.CodeAnalysis;

namespace TC.CloudGames.Application.Middleware
{
    [ExcludeFromCodeCoverage]
    public class CommandPreProcessor<TRequest> : IPreProcessor<TRequest>
    {
        public Task PreProcessAsync(IPreProcessorContext<TRequest> context, CancellationToken ct)
        {
            var logger = context.HttpContext.Resolve<ILogger<TRequest>>();
            var name = context.Request!.GetType().Name;

            using (LogContext.PushProperty("RequestContent", context.Request, true))
            {
                logger.LogInformation("Pre-processing request: {Request}", name);
            }

            if (context.HasValidationFailures)
            {
                using (LogContext.PushProperty("Error", context.ValidationFailures, true))
                {
                    logger.LogError("Pre-processing Request {Request} validation failed with error", name);
                }
                return Task.CompletedTask;
            }

            logger.LogInformation("Pre-processing Request {Request} executed successfully", name);

            return Task.CompletedTask;
        }
    }
}
