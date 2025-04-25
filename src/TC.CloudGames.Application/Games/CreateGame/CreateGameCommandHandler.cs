using Ardalis.Result;
using TC.CloudGames.Application.Abstractions.Messaging;
using TC.CloudGames.Domain.Abstractions;
using TC.CloudGames.Domain.Exceptions;
using TC.CloudGames.Domain.Game;

namespace TC.CloudGames.Application.Games.CreateGame
{
    internal sealed class CreateGameCommandHandler : ICommandHandler<CreateGameCommand, CreateGameResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGameRepository _gameRepository;

        public CreateGameCommandHandler(IUnitOfWork unitOfWork, IGameRepository gameRepository)
        {
            _unitOfWork = unitOfWork;
            _gameRepository = gameRepository;
        }
        public async Task<Result<CreateGameResponse>> ExecuteAsync(CreateGameCommand command, CancellationToken ct)
        {
            var entity = CreateGameMapper.ToEntity(command);
            if (!entity.IsSuccess)
            {
                return Result<CreateGameResponse>.Error(new ErrorList(entity.Errors));
            }

            try
            {
                _gameRepository.Add(entity);
                await _unitOfWork.SaveChangesAsync(ct).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is IDuplicateKeyException duplicateEx)
            {
                return Result<CreateGameResponse>.Error(
                    $"Registro duplicado na tabela '{duplicateEx.TableName}'. Restrição violada: '{duplicateEx.ConstraintName}'");
            }

            var response = CreateGameMapper.FromEntity(entity);
            return Result<CreateGameResponse>.Success(response);
        }
    }
}
