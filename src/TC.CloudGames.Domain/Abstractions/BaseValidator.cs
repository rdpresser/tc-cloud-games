using Ardalis.Result;
using FluentValidation;
using FluentValidation.Results;

namespace TC.CloudGames.Domain.Abstractions
{
    public abstract class BaseValidator<TEntity> : AbstractValidator<TEntity>, IBaseValidator<TEntity>
        where TEntity : class
    {
        public async Task<ValidationResult> ValidationResultAsync(TEntity entity)
        {
            var context = new ValidationContext<TEntity>(entity);
            return await base.ValidateAsync(context);
        }

        public ValidationResult ValidationResult(TEntity entity)
        {
            var context = new ValidationContext<TEntity>(entity);
            return base.Validate(context);
        }

        public IEnumerable<ValidationError> ValidationErrors(TEntity entity)
        {
            var context = new ValidationContext<TEntity>(entity);
            var validation = base.Validate(context);

            if (!validation.IsValid)
            {
                return validation.Errors.Select(e => new ValidationError
                {
                    Identifier = e.PropertyName,
                    ErrorMessage = e.ErrorMessage,
                    ErrorCode = e.ErrorCode,
                    Severity = (ValidationSeverity)e.Severity,
                });
            }

            return Enumerable.Empty<ValidationError>();
        }

        public IEnumerable<ValidationError> ValidationErrors(ValidationResult validationResult)
        {
            return validationResult.Errors.Select(e => new ValidationError
            {
                Identifier = e.PropertyName,
                ErrorMessage = e.ErrorMessage,
                ErrorCode = e.ErrorCode,
                Severity = (ValidationSeverity)e.Severity,
            });
        }

        public IDictionary<string, string[]> Errors(ValidationResult validationResult)
        {
            return validationResult.ToDictionary();
        }
    }
}
