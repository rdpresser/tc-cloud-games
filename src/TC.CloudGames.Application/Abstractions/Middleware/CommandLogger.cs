using FastEndpoints;
using Microsoft.Extensions.Logging;

namespace TC.CloudGames.Application.Abstractions.Middleware
{
    public sealed class CommandLogger<TCommand, TResult> : ICommandMiddleware<TCommand, TResult>
        where TCommand : ICommand<TResult> // Added 'class' constraint to ensure TCommand is a reference type
    {
        private readonly ILogger<CommandLogger<TCommand, TResult>> logger;

        public CommandLogger(ILogger<CommandLogger<TCommand, TResult>> logger)
        {
            this.logger = logger;
        }

        public async Task<TResult> ExecuteAsync(TCommand command, CommandDelegate<TResult> next, CancellationToken ct)
        {
            logger.LogInformation("Executing command: {name}", command.GetType().Name);

            var result = await next();

            logger.LogInformation("Got result: {value}", result);

            return result;
        }
    }
}
