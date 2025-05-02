using Ardalis.Result;
using TC.CloudGames.Application.Abstractions.Messaging;

namespace TC.CloudGames.Application.Users.GetUserList;

internal sealed class GetUserListQueryHandler : QueryHandler<GetUserListQuery, IReadOnlyList<UserListResponse>>
{
    private readonly IUserPgRepository _userRepository;

    public GetUserListQueryHandler(IUserPgRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public override async Task<Result<IReadOnlyList<UserListResponse>>> ExecuteAsync(GetUserListQuery query,
        CancellationToken ct = default)
    {
        var users = await _userRepository.GetUserListAsync(query, ct);
        if (users is null || !users.Any()) return Result<IReadOnlyList<UserListResponse>>.Success([]);

        return Result.Success<IReadOnlyList<UserListResponse>>([.. users]);
    }
}