using Ardalis.Result;
using TC.CloudGames.Application.Abstractions.Messaging;
using TC.CloudGames.CrossCutting.Commons.Clock;
using TC.CloudGames.Domain.Abstractions;
using TC.CloudGames.Domain.Game;

namespace TC.CloudGames.Application.Games.CreateGame;

internal sealed class CreateGameCommandHandler : CommandHandler<CreateGameCommand, CreateGameResponse, Game, IGameEfRepository>
{
    private readonly IDateTimeProvider _dateTimeProvider;
    public CreateGameCommandHandler(IUnitOfWork unitOfWork, IGameEfRepository gameRepository, IDateTimeProvider dateTimeProvider)
        : base(unitOfWork, gameRepository)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public override async Task<Result<CreateGameResponse>> ExecuteAsync(CreateGameCommand command,
        CancellationToken ct = default)
    {
        var entity = CreateGameMapper.ToEntity(command, _dateTimeProvider);
        if (!entity.IsSuccess)
        {
            AddErrors(entity.ValidationErrors);
            return Result<CreateGameResponse>.Invalid(entity.ValidationErrors);
        }

        Repository.Add(entity);

        await UnitOfWork.SaveChangesAsync(ct).ConfigureAwait(false);

        return CreateGameMapper.FromEntity(entity);
    }
}