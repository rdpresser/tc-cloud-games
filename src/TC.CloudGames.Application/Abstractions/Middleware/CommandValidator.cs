using Ardalis.Result;
using FastEndpoints;
using Microsoft.Extensions.Logging;

namespace TC.CloudGames.Application.Abstractions.Middleware
{
    public sealed class CommandValidator<TCommand, TResult>(ILogger<TCommand> logger, IEnumerable<FluentValidation.IValidator<TCommand>> validators) : ICommandMiddleware<TCommand, TResult>
        where TCommand : ICommand<TResult>
    {
        public async Task<TResult> ExecuteAsync(TCommand command, CommandDelegate<TResult> next, CancellationToken ct)
        {
            if (!validators.Any())
            {
                return await next();
            }
            //var context = FastEndpoints.ValidationContext<TCommand>.Instance;
            //var context = new FluentValidation.ValidationContext<TCommand>(command);
            var context = new FluentValidation.ValidationContext<TCommand>(command);

            var validationErrors = validators
                .Select(validator => validator.Validate(context))
                .Where(validationResult => validationResult.Errors.Any())
                .SelectMany(validationResult => validationResult.Errors)
                .Select(validationFailure => new ValidationError(
                    validationFailure.PropertyName,
                    validationFailure.ErrorMessage))
                .ToList();

            if (validationErrors.Any())
            {
                throw new Exceptions.ValidationException(validationErrors);
            }

            return await next();
        }
    }
}
