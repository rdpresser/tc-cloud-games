using Ardalis.Result;

namespace TC.CloudGames.Application.Abstractions.Messaging
{
    public interface IQuery<TResponse> : FastEndpoints.ICommand<Result<TResponse>>
    {
    }
}
