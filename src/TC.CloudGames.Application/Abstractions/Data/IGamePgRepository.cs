using TC.CloudGames.Application.Games.GetGame;
using TC.CloudGames.Application.Games.GetGameList;

namespace TC.CloudGames.Application.Abstractions.Data;

public interface IGamePgRepository
{
    Task<GameByIdResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GameListResponse>> GetGameListAsync(GetGameListQuery query, CancellationToken cancellationToken = default);
}