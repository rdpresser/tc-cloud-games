using Ardalis.Result;

namespace TC.CloudGames.Application.Abstractions.Messaging
{
    public abstract class QueryHandler<TQuery, TResponse> : FastEndpoints.CommandHandler<TQuery, Result<TResponse>>
            where TQuery : IQuery<TResponse>
            where TResponse : class
    {
        private FastEndpoints.ValidationContext<TQuery> ValidationContext { get; } = Instance;

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
