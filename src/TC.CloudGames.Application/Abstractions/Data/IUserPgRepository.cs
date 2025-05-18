using TC.CloudGames.Application.Users.GetUserByEmail;
using TC.CloudGames.Application.Users.GetUserById;
using TC.CloudGames.Application.Users.GetUserList;

namespace TC.CloudGames.Application.Abstractions.Data;

public interface IUserPgRepository
{
    Task<UserByIdResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserListResponse>> GetUserListAsync(GetUserListQuery query, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<UserByEmailResponse?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}