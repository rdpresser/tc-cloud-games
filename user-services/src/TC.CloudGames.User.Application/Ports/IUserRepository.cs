using System;
using System.Collections.Generic;
using System.Threading;
using TC.CloudGames.User.Domain.Aggregates;

namespace TC.CloudGames.User.Application.Ports
{
    public interface IUserRepository
    {
        Task<UserAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task SaveAsync(UserAggregate user, CancellationToken cancellationToken = default);
        Task<IEnumerable<UserAggregate>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}
