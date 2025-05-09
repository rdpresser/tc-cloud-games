using TC.CloudGames.Domain.Abstractions;

namespace TC.CloudGames.Domain.User;

public interface IUserEfRepository : IEfRepository<User>
{
    Task<User?> GetByEmailWithPasswordAsync(string email, string password,
        CancellationToken cancellationToken = default);

    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
}