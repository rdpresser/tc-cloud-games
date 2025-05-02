using TC.CloudGames.Application.Games.GetGame;
using TC.CloudGames.Application.Games.GetGameList;

namespace TC.CloudGames.Application.Games;

public interface IGamePgRepository
{
    Task<GameResponse?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<IReadOnlyList<GameListResponse>> GetGameListAsync(GetGameListQuery query,
        CancellationToken cancellationToken = default);
}