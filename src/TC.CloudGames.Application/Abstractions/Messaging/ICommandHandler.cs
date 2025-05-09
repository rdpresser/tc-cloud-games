using Ardalis.Result;

namespace TC.CloudGames.Application.Abstractions.Messaging
{
    public interface ICommandHandler<in TCommand> : FastEndpoints.ICommandHandler<TCommand, Result>
        where TCommand : ICommand
    {
    }

    public interface ICommandHandler<in TCommand, TResponse> : FastEndpoints.ICommandHandler<TCommand, Result<TResponse>>
        where TCommand : ICommand<TResponse>
    {
    }
}