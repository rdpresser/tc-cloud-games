using Ardalis.Result;
using FluentValidation;
using FluentValidation.Results;
using System.Linq.Expressions;
using TC.CloudGames.Application.Abstractions.Data;
using TC.CloudGames.Domain.Abstractions;
using TC.CloudGames.Domain.Exceptions;

namespace TC.CloudGames.Application.Abstractions.Messaging
{
    public abstract class CommandHandler<TCommand, TResponse, TEntity, TRepository> : FastEndpoints.CommandHandler<TCommand, Result<TResponse>>
        where TCommand : ICommand<TResponse>
        where TResponse : class
        where TEntity : Entity
        where TRepository : IEfRepository<TEntity>
    {
        protected readonly TRepository Repository;
        protected readonly IUnitOfWork UnitOfWork;

        protected CommandHandler(IUnitOfWork unitOfWork, TRepository repository)
        {
            UnitOfWork = unitOfWork;
            Repository = repository;
        }

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

        /// <summary>
        /// Adds a list of validation errors to the context.
        /// </summary>
        /// <param name="validations"></param>
        protected void AddErrors(IEnumerable<ValidationError> validations)
        {
            ValidationContext.ValidationFailures.AddRange(validations.Select(validation =>
                new ValidationFailure
                {
                    PropertyName = validation.Identifier,
                    ErrorMessage = validation.ErrorMessage,
                    ErrorCode = validation.ErrorCode,
                    Severity = (Severity)validation.Severity
                }));
        }


        /// <summary>
        /// Creates a result with validation errors to Result<typeparamref name="TResponse"/>.
        /// </summary>
        /// <returns></returns>
        protected Result<TResponse> ValidationErrorsInvalid()
        {
            if (ValidationContext.ValidationFailures.Count == 0)
            {
                return Result<TResponse>.Success(default!);
            }

            List<ValidationError> validationErrors = [];

            validationErrors.AddRange(ValidationContext.ValidationFailures
                .Select(x => new ValidationError
                {
                    Identifier = x.PropertyName,
                    ErrorCode = x.ErrorCode,
                    ErrorMessage = x.ErrorMessage,
                    Severity = (ValidationSeverity)x.Severity
                }));

            return Result<TResponse>.Invalid(validationErrors);
        }

        /// <summary>
        /// Creates a result with not found error messages.
        /// </summary>
        /// <returns></returns>
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