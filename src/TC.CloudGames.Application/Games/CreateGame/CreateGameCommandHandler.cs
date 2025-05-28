using TC.CloudGames.Application.Abstractions.Data;
using TC.CloudGames.Domain.GameAggregate;
using TC.CloudGames.Domain.GameAggregate.Abstractions;

namespace TC.CloudGames.Application.Games.CreateGame;

internal sealed class CreateGameCommandHandler : CommandHandler<CreateGameCommand, CreateGameResponse, Game, IGameEfRepository>
{
    public CreateGameCommandHandler(IUnitOfWork unitOfWork, IGameEfRepository gameRepository)
        : base(unitOfWork, gameRepository)
    {

    }

    public override async Task<Result<CreateGameResponse>> ExecuteAsync(CreateGameCommand command,
        CancellationToken ct = default)
    {
        var entity = CreateGameMapper.ToEntity(command);

        if (!entity.IsSuccess)
        {
            AddErrors(entity.ValidationErrors);
            return Result.Invalid(entity.ValidationErrors);
        }

        Repository.Add(entity);

        await UnitOfWork.SaveChangesAsync(ct).ConfigureAwait(false);

        return CreateGameMapper.FromEntity(entity);
    }
}