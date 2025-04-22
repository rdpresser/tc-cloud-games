using Ardalis.Result;
using FastEndpoints;
using TC.CloudGames.CrossCutting.Commons.Logger;

namespace TC.CloudGames.Application.Middleware
{
    public sealed class CommandLogger<TCommand, TResult> : ICommandMiddleware<TCommand, TResult>
        where TCommand : ICommand<TResult> // Added 'class' constraint to ensure TCommand is a reference type
    {
        private readonly BaseLogger<CommandLogger<TCommand, TResult>> _logger;

        public CommandLogger(BaseLogger<CommandLogger<TCommand, TResult>> logger)
        {
            _logger = logger;
        }

        public async Task<TResult> ExecuteAsync(TCommand command, CommandDelegate<TResult> next, CancellationToken ct)
        {
            _logger.LogInformation($"Executing command: '{command.GetType().Name}'");

            var result = await next();

            if (result is IResult resultWithErrors && resultWithErrors.Errors.Any())
            {
                _logger.LogError($"Command '{command.GetType().Name}' failed with errors: '{string.Join(", ", resultWithErrors.Errors)}'");
            }
            else
            {
                _logger.LogInformation($"Command '{command.GetType().Name}' executed successfully");
            }
            return result;
        }
    }
}
