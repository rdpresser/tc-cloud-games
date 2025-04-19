using FastEndpoints;
using Microsoft.Extensions.Logging;

namespace TC.CloudGames.Application.Abstractions.Middleware
{
    public sealed class CommandLogger<TCommand, TResult>(ILogger<TCommand> logger) : ICommandMiddleware<TCommand, TResult>
        where TCommand : ICommand<TResult>
    {
        public async Task<TResult> ExecuteAsync(TCommand command, CommandDelegate<TResult> next, CancellationToken ct)
        {
            logger.LogInformation("Executing command: {name}", command.GetType().Name);

            var result = await next();

            logger.LogInformation("Got result: {value}", result);

            return result;
        }
    }
}
