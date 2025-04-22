using Ardalis.Result;
using FastEndpoints;
using TC.CloudGames.CrossCutting.Commons.Logger;

namespace TC.CloudGames.Application.Middleware
{
    public sealed class CommandLogger<TCommand, TResult> : ICommandMiddleware<TCommand, TResult>
            where TCommand : ICommand<TResult>
    {
        private readonly BaseLogger<CommandLogger<TCommand, TResult>> _logger;

        // Internal list of all concrete classes that inherit from ICommand<>
        //private readonly IReadOnlyList<Type> _commandTypes;

        public CommandLogger(BaseLogger<CommandLogger<TCommand, TResult>> logger)
        {
            _logger = logger;

            //// Use reflection to find all concrete types implementing ICommand<>
            //_commandTypes = Assembly.GetExecutingAssembly()
            //                        .GetTypes()
            //                        .Where(t => !t.IsAbstract && typeof(Abstractions.Messaging.IBaseCommand).IsAssignableFrom(t))
            //                        .ToList()
            //                        .AsReadOnly();
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

            //// Verify if the result is assignable to any type in _commandTypes list
            //if (_commandTypes.Any(type => type.IsAssignableFrom(command.GetType())))
            //{
            //    if (result is Result<TResult> castedResult)
            //    {
            //        if (castedResult.Errors.Any())
            //        {
            //            _logger.LogError($"Command '{command.GetType().Name}' failed with errors: '{string.Join(", ", castedResult.Errors)}'");
            //        }
            //        else
            //        {
            //            _logger.LogInformation($"Command '{command.GetType().Name}' executed successfully with result: '{castedResult.Value}'");
            //        }
            //    }
            //}
            //else if (result is IResult resultWithErrors && resultWithErrors.Errors.Any())
            //{
            //    _logger.LogError($"Command '{command.GetType().Name}' failed with errors: '{string.Join(", ", resultWithErrors.Errors)}'");
            //}
            //else
            //{
            //    _logger.LogInformation($"Command '{command.GetType().Name}' executed successfully");
            //}

            return result;
        }
    }
}
