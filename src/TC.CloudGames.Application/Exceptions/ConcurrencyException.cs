using System.Diagnostics.CodeAnalysis;

namespace TC.CloudGames.Application.Exceptions
{
    [ExcludeFromCodeCoverage]
    public sealed class ConcurrencyException : Exception
    {
        public ConcurrencyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
