using TC.CloudGames.Application.Abstractions.Data;

namespace TC.CloudGames.Application.Games.GetGameList;

internal sealed class GetGameListQueryHandler : QueryHandler<GetGameListQuery, IReadOnlyList<GameListResponse>>
{
    private readonly IGamePgRepository _gameRepository;

    public GetGameListQueryHandler(IGamePgRepository gameRepository)
    {
        _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
    }

    public override async Task<Result<IReadOnlyList<GameListResponse>>> ExecuteAsync(GetGameListQuery query,
        CancellationToken ct = default)
    {
        var games = await _gameRepository
            .GetGameListAsync(query, ct)
            .ConfigureAwait(false);

        if (games is null || !games.Any())
            return Result<IReadOnlyList<GameListResponse>>.Success([]);

        return Result.Success<IReadOnlyList<GameListResponse>>([.. games]);
    }
}