using Marten;
using Marten.Events.Projections;
using TC.CloudGames.User.Domain.Aggregates;

namespace TC.CloudGames.User.Infrastructure.Projections;

public class UserProjectionHandler : EventProjection
{
    public void Project(UserCreatedEvent @event, IDocumentOperations operations)
    {
        var projection = new UserProjection
        {
            Id = @event.Id,
            Email = @event.Email,
            Username = @event.Username,
            CreatedAt = @event.CreatedAt,
            IsActive = true
        };
        operations.Store(projection);
    }

    public void Project(UserUpdatedEvent @event, IDocumentOperations operations)
    {
        var projection = new UserProjection
        {
            Id = @event.Id,
            Email = @event.Email,
            Username = @event.Username,
            UpdatedAt = @event.UpdatedAt,
            IsActive = true // Assume still active unless deactivated event is processed
        };
        operations.Store(projection);
    }

    public void Project(UserDeactivatedEvent @event, IDocumentOperations operations)
    {
        var projection = new UserProjection
        {
            Id = @event.Id,
            IsActive = false,
            UpdatedAt = @event.DeactivatedAt
        };
        operations.Store(projection);
    }
}