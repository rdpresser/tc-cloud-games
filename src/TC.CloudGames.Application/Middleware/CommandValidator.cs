using System.Diagnostics.CodeAnalysis;

namespace TC.CloudGames.Application.Middleware
{
    [ExcludeFromCodeCoverage]
    public sealed class CommandValidator<TCommand, TResult> : ICommandMiddleware<TCommand, TResult>
        where TCommand : FastEndpoints.ICommand<TResult>
    {
        private readonly IEnumerable<AbstractValidator<TCommand>> _validators;

        public CommandValidator(IEnumerable<AbstractValidator<TCommand>> validators)
        {
            _validators = validators;
        }

        public async Task<TResult> ExecuteAsync(TCommand command, CommandDelegate<TResult> next, CancellationToken ct)
        {
            ArgumentNullException.ThrowIfNull(next);

            if (!_validators.Any())
            {
                return await next().ConfigureAwait(false);
            }

            var context = new FluentValidation.ValidationContext<TCommand>(command);

            var validationErrors = _validators
                .Select(validator => validator.Validate(context))
                .Where(validationResult => validationResult.Errors.Count != 0)
                .SelectMany(validationResult => validationResult.Errors)
                .Select(validationFailure => new ValidationError(
                    validationFailure.PropertyName,
                    validationFailure.ErrorMessage))
                .ToList();

            if (validationErrors.Count != 0)
            {
                throw new Exceptions.ValidationException(validationErrors);
            }

            return await next().ConfigureAwait(false);
        }
    }
}
