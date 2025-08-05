using Marten;
using TC.CloudGames.User.Application.Ports;
using TC.CloudGames.User.Domain.Aggregates;
using TC.CloudGames.User.Infrastructure.Projections;

namespace TC.CloudGames.User.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDocumentSession _session;

    public UserRepository(IDocumentSession session)
    {
        _session = session ?? throw new ArgumentNullException(nameof(session));
    }

    public async Task<UserAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _session.Events.AggregateStreamAsync<UserAggregate>(id, token: cancellationToken);
    }

    public async Task SaveAsync(UserAggregate user, CancellationToken cancellationToken = default)
    {
        if (user.UncommittedEvents.Any())
        {
            // Check if this is a new aggregate by reusing GetByIdAsync
            var existingAggregate = await GetByIdAsync(user.Id, cancellationToken);
            
            if (existingAggregate == null)
            {
                // For new aggregates, start a new stream
                _session.Events.StartStream<UserAggregate>(user.Id, user.UncommittedEvents.ToArray());
            }
            else
            {
                // For existing aggregates, append events to the existing stream
                _session.Events.Append(user.Id, user.UncommittedEvents.ToArray());
            }
            
            await _session.SaveChangesAsync(cancellationToken);
            user.MarkEventsAsCommitted();
        }
    }

    public async Task<IEnumerable<UserAggregate>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        // Efficient approach: Use projections directly instead of replaying events
        // Since projections are always up-to-date, this avoids N+1 queries and unnecessary event replay
        var userProjections = await _session.Query<UserProjection>()
            .Where(u => u.IsActive)
            .ToListAsync(cancellationToken);

        return userProjections.Select(projection => 
            UserAggregate.FromProjection(
                projection.Id, 
                projection.Email, 
                projection.Username, 
                projection.CreatedAt, 
                projection.UpdatedAt, 
                projection.IsActive));
    }
}