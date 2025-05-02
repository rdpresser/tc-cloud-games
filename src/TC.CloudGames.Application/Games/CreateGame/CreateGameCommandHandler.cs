using Ardalis.Result;
using TC.CloudGames.Application.Abstractions.Messaging;
using TC.CloudGames.Domain.Abstractions;
using TC.CloudGames.Domain.Game;

namespace TC.CloudGames.Application.Games.CreateGame;

internal sealed class CreateGameCommandHandler : CommandHandler<CreateGameCommand, CreateGameResponse>
{
    private readonly IGameEfRepository _gameRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateGameCommandHandler(IUnitOfWork unitOfWork, IGameEfRepository gameRepository)
    {
        _unitOfWork = unitOfWork;
        _gameRepository = gameRepository;
    }

    public override async Task<Result<CreateGameResponse>> ExecuteAsync(CreateGameCommand command,
        CancellationToken ct = default)
    {
        var entity = CreateGameMapper.ToEntity(command);
        if (!entity.IsSuccess)
        {
            AddErrors(entity.ValidationErrors);
            return Result<CreateGameResponse>.Invalid(entity.ValidationErrors);
        }

        _gameRepository.Add(entity);

        await _unitOfWork.SaveChangesAsync(ct).ConfigureAwait(false);

        var response = CreateGameMapper.FromEntity(entity);
        return Result<CreateGameResponse>.Success(response);
    }
}