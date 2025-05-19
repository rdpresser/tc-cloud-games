using System.Diagnostics.CodeAnalysis;

namespace TC.CloudGames.Application.Exceptions
{
    [ExcludeFromCodeCoverage]
    public sealed class ValidationException : Exception
    {
        public ValidationException(IEnumerable<ValidationError> errors)
            : base("One or more validation failures have occurred.")
        {
            Errors = errors;
        }

        public IEnumerable<ValidationError> Errors { get; }
    }
}
