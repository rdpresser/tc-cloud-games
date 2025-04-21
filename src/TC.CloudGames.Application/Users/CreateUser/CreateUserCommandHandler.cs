using Ardalis.Result;
using TC.CloudGames.Domain.Abstractions;
using TC.CloudGames.Domain.Exceptions;
using TC.CloudGames.Domain.User;

namespace TC.CloudGames.Application.Users.CreateUser;

internal sealed class CreateUserCommandHandler : Abstractions.Messaging.ICommandHandler<CreateUserCommand, CreateUserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CreateUserMapper _mapper;

    public CreateUserCommandHandler(CreateUserMapper mapper, IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<CreateUserResponse>> ExecuteAsync(CreateUserCommand command, CancellationToken ct)
    {
        var entity = _mapper.ToEntity(command);

        try
        {
            _userRepository.Add(entity);

            await _unitOfWork.SaveChangesAsync(ct);
        }
        catch (Exception ex) when (ex is IDuplicateKeyException duplicateEx)
        {
            return Result<CreateUserResponse>.Error(
                $"Registro duplicado na tabela '{duplicateEx.TableName}'. Restrição violada: '{duplicateEx.ConstraintName}'");
        }

        var response = _mapper.FromEntity(entity);
        return Result<CreateUserResponse>.Success(response);
    }
}