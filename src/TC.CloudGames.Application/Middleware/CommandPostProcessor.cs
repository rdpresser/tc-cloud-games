using Serilog.Context;
using System.Diagnostics.CodeAnalysis;

namespace TC.CloudGames.Application.Middleware
{
    [ExcludeFromCodeCoverage]
    public class CommandPostProcessor<TRequest, TResponse> : IPostProcessor<TRequest, TResponse>
        where TRequest : Abstractions.Messaging.ICommand<TResponse>
        where TResponse : class
    {
        public Task PostProcessAsync(IPostProcessorContext<TRequest, TResponse> context, CancellationToken ct)
        {
            var logger = context.HttpContext.Resolve<ILogger<TRequest>>();

            var genericType = logger.GetType().GenericTypeArguments.FirstOrDefault()?.Name ?? "Unknown";
            var name = context.Request?.GetType().Name
                       ?? genericType;

            if (!context.HasValidationFailures)
            {
                var responseValues = new
                {
                    context.Request,
                    context.Response,
                };

                using (LogContext.PushProperty("Content", responseValues, true))
                {
                    logger.LogInformation("Post-processing Request {Request} executed successfully", name);
                }
            }
            else
            {
                var responseValues = new
                {
                    context.Request,
                    context.Response,
                    Error = context.ValidationFailures
                };

                using (LogContext.PushProperty("Content", responseValues, true))
                {
                    logger.LogError("Post-processing Request {Request} validation failed with error", name);
                }
            }

            return Task.CompletedTask;
        }
    }
}
