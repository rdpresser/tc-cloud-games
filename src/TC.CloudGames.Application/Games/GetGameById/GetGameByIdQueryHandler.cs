using TC.CloudGames.Application.Abstractions.Data;
using TC.CloudGames.Domain.Game.Abstractions;

namespace TC.CloudGames.Application.Games.GetGameById
{
    internal sealed class GetGameByIdQueryHandler : QueryHandler<GetGameByIdQuery, GameByIdResponse>
    {
        private readonly IGamePgRepository _gameRepository;

        public GetGameByIdQueryHandler(IGamePgRepository gameRepository)
        {
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
        }

        public override async Task<Result<GameByIdResponse>> ExecuteAsync(GetGameByIdQuery command, CancellationToken ct)
        {
            var result = await _gameRepository.GetByIdAsync(command.Id, ct).ConfigureAwait(false);
            if (result is not null)
                return result;

            AddError(x => x.Id, $"Game with id '{command.Id}' not found.", GameDomainErrors.NotFound.ErrorCode);
            return ValidationErrorNotFound();
        }
    }
}
