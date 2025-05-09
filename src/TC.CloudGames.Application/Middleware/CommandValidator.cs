using Ardalis.Result;
using FastEndpoints;
using FluentValidation;

namespace TC.CloudGames.Application.Middleware
{
    public sealed class CommandValidator<TCommand, TResult> : ICommandMiddleware<TCommand, TResult>
        where TCommand : ICommand<TResult>
    {
        private readonly IEnumerable<AbstractValidator<TCommand>> _validators;

        public CommandValidator(IEnumerable<AbstractValidator<TCommand>> validators)
        {
            _validators = validators;
        }

        public async Task<TResult> ExecuteAsync(TCommand command, CommandDelegate<TResult> next, CancellationToken ct)
        {
            if (!_validators.Any())
            {
                return await next();
            }

            var context = new FluentValidation.ValidationContext<TCommand>(command);

            var validationErrors = _validators
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
