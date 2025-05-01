using Ardalis.Result;
using FluentValidation;
using FluentValidation.Results;
using System.Linq.Expressions;
using TC.CloudGames.Domain.Exceptions;

namespace TC.CloudGames.Application.Abstractions.Messaging
{
    public abstract class CommandHandler<TCommand, TResponse> : FastEndpoints.CommandHandler<TCommand, Result<TResponse>>
        where TCommand : ICommand<TResponse>
        where TResponse : class
    {
        private FastEndpoints.ValidationContext<TCommand> ValidationContext { get; } = Instance;

        public abstract override Task<Result<TResponse>> ExecuteAsync(TCommand command, CancellationToken ct = default);

        protected Result<TResponse> HandleDuplicateKeyException(IDuplicateKeyException exception)
        {
            AddError(
                $"Table: {exception.TableName}",
                $"Duplicate record in table '{exception.TableName}'. Violated constraint: '{exception.ConstraintName}'",
                $"Violated constraint: {exception.ConstraintName}"
            );

            return ValidationErrorsInvalid();
        }

        protected new void AddError(Expression<Func<TCommand, object?>> property, string errorMessage,
            string? errorCode = null, Severity severity = Severity.Error)
        {
            ValidationContext.AddError(property, errorMessage, errorCode, severity);
        }

        protected void AddError(string property, string errorMessage, string? errorCode = null,
            Severity severity = Severity.Error)
        {
            ValidationContext.AddError(new ValidationFailure
            {
                PropertyName = property,
                ErrorMessage = errorMessage,
                ErrorCode = errorCode,
                Severity = severity
            });
        }

        protected void AddErrors(IEnumerable<ValidationError> validations)
        {
            validations.ToList().ForEach(validation =>
            {
                ValidationContext.AddError(new()
                {
                    PropertyName = validation.Identifier,
                    ErrorMessage = validation.ErrorMessage,
                    ErrorCode = validation.ErrorCode,
                    Severity = (Severity)validation.Severity
                });
            });
        }

        protected Result<TResponse> ValidationErrorsInvalid()
        {
            if (ValidationContext.ValidationFailures.Count == 0)
            {
                return Result<TResponse>.Success(default!);
            }

            List<ValidationError> validationErrors = [];

            ValidationContext.ValidationFailures.ForEach(error =>
            {
                validationErrors.Add(new ValidationError
                {
                    Identifier = error.PropertyName,
                    ErrorCode = error.ErrorCode,
                    ErrorMessage = error.ErrorMessage,
                    Severity = (ValidationSeverity)error.Severity
                });
            });

            return Result<TResponse>.Invalid(validationErrors);
        }

        protected Result<TResponse> ValidationErrorNotFound()
        {
            if (ValidationContext.ValidationFailures.Count == 0)
            {
                return Result<TResponse>.Success(default!);
            }

            return Result<TResponse>.NotFound([.. ValidationContext.ValidationFailures.Select(x => x.ErrorMessage)]);
        }
    }
}