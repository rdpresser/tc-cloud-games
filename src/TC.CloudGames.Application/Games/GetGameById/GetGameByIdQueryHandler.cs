using Ardalis.Result;
using TC.CloudGames.Application.Abstractions.Messaging;
using TC.CloudGames.Domain.Game;

namespace TC.CloudGames.Application.Games.GetGame
{
    internal sealed class GetGameByIdQueryHandler : QueryHandler<GetGameByIdQuery, GameResponse>
    {
        private readonly IGamePgRepository _gameRepository;

        public GetGameByIdQueryHandler(IGamePgRepository gameRepository)
        {
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
        }

        public override async Task<Result<GameResponse>> ExecuteAsync(GetGameByIdQuery command, CancellationToken ct)
        {
            var result = await _gameRepository.GetByIdAsync(command.Id, ct);
            if (result is not null)
                return result;

            AddError(x => x.Id, $"Game with id '{command.Id}' not found.", GameDomainErrors.NotFound.ErrorCode);
            return ValidationErrorNotFound();
        }
    }
}
