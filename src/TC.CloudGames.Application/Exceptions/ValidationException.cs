using System.Diagnostics.CodeAnalysis;

namespace TC.CloudGames.Application.Exceptions
{
    [ExcludeFromCodeCoverage]
    public sealed class ValidationException : Exception
    {
        public IEnumerable<ValidationError> Errors { get; }

        public ValidationException()
            : base("One or more validation failures have occurred.")
        {
            Errors = [];
        }

        public ValidationException(string message)
            : base(message)
        {
            Errors = [new(message)];
        }

        public ValidationException(string message, Exception innerException)
            : base(message, innerException)
        {
            Errors = [new(message)];
        }

        public ValidationException(IEnumerable<ValidationError> errors)
            : base("One or more validation failures have occurred.")
        {
            Errors = errors;
        }
    }
}
