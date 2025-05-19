using System.Diagnostics.CodeAnalysis;

namespace TC.CloudGames.Application.Exceptions
{
    [ExcludeFromCodeCoverage]
    public static class ValidationExtensions
    {
        public static void AddValidationErrors<TCommand, TResult>(this IEnumerable<ValidationError> validationErrors, Endpoint<TCommand, TResult> endpoint)
            where TCommand : class
            where TResult : class
        {
            validationErrors.ToList().ForEach(error =>
            {
                endpoint.AddError(new ValidationFailure(error.Identifier, error.ErrorMessage)
                {
                    ErrorCode = error.ErrorCode,
                    Severity = (FluentValidation.Severity)error.Severity
                });
            });
        }
    }
}