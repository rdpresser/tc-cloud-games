using TC.CloudGames.Application.Users.GetUser;
using TC.CloudGames.Application.Users.GetUserList;

namespace TC.CloudGames.Application.Users;

public interface IUserPgRepository
{
    Task<UserResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserListResponse>> GetUserListAsync(GetUserListQuery query, CancellationToken cancellationToken = default);
}