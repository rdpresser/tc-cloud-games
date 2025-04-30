namespace TC.CloudGames.Application.Abstractions.Messaging
{
    public abstract class QueryHandler<TQuery, TResponse> : CommandHandler<TQuery, TResponse>
            where TQuery : IQuery<TResponse>
            where TResponse : class
    {
    }
}
