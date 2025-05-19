using Serilog.Context;
using System.Diagnostics.CodeAnalysis;

namespace TC.CloudGames.Application.Middleware
{
    [ExcludeFromCodeCoverage]
    public sealed class CommandLogger<TCommand, TResult> : ICommandMiddleware<TCommand, TResult>
            where TCommand : FastEndpoints.ICommand<TResult>
            where TResult : class, IResult
    {
        private readonly ILogger<CommandLogger<TCommand, TResult>> _logger;

        public CommandLogger(ILogger<CommandLogger<TCommand, TResult>> logger)
        {
            _logger = logger;
        }

        private void LogResponseIfApplicable<TResponse>(TResponse result, string requestName)
        {
            var resultType = result!.GetType();
            var genericResultType = typeof(Result<>);

            if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == genericResultType)
            {
                var valueProperty = resultType.GetProperty("Value");
                if (valueProperty != null)
                {
                    var value = valueProperty.GetValue(result);
                    using (LogContext.PushProperty($"{resultType.GenericTypeArguments[0].Name}", value, true))
                    {
                        _logger.LogInformation("Request {Request} executed successfully", requestName);
                    }
                }
            }
            else
            {
                _logger.LogInformation("Request {Request} executed successfully", requestName);
            }
        }

        public async Task<TResult> ExecuteAsync(TCommand command, CommandDelegate<TResult> next, CancellationToken ct)
        {
            var name = command.GetType().Name;

            try
            {
                using (LogContext.PushProperty("RrequestContent", command, true))
                {
                    _logger.LogInformation("Executing request: {Request}", name);
                }

                var result = await next();

                if (!result.IsOk())
                {
                    if (result.ValidationErrors.Any())
                    {
                        using (LogContext.PushProperty("Error", result.ValidationErrors, true))
                        {
                            _logger.LogError("Request {Request} processing failed with error", name);
                        }
                    }
                    else if (result.Errors.Any())
                    {
                        using (LogContext.PushProperty("Error", result.Errors, true))
                        {
                            _logger.LogError("Request {Request} processing failed with error", name);
                        }
                    }
                    else
                    {
                        _logger.LogError("Request {Request} processing failed with unknown error", name);
                    }
                }
                else
                {
                    LogResponseIfApplicable(result, name);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Request {Request} processing failed", name);

                throw;
            }
        }
    }
}
