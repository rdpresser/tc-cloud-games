using TC.CloudGames.Application.Users.GetUser;
using TC.CloudGames.Application.Users.GetUserList;

namespace TC.CloudGames.Application.Abstractions.Data;

public interface IUserPgRepository
{
    Task<UserResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserListResponse>> GetUserListAsync(GetUserListQuery query, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<UserResponse?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}