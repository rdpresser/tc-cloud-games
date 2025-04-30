using Ardalis.Result;
using TC.CloudGames.Application.Abstractions.Messaging;
using TC.CloudGames.Domain.Abstractions;
using TC.CloudGames.Domain.Exceptions;
using TC.CloudGames.Domain.User;

namespace TC.CloudGames.Application.Users.CreateUser;

internal sealed class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, CreateUserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreateUserResponse>> ExecuteAsync(CreateUserCommand command, CancellationToken ct)
    {
        var entity = CreateUserMapper.ToEntity(command);

        try
        {
            // Check if the email already exists
            if (await _userRepository.EmailExistsAsync(command.Email, ct).ConfigureAwait(false))
            {
                return Result<CreateUserResponse>.Error("Email already exists.");
            }
            
            _userRepository.Add(entity);

            await _unitOfWork.SaveChangesAsync(ct).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is IDuplicateKeyException duplicateEx)
        {
            return Result<CreateUserResponse>.Error(
                $"Registro duplicado na tabela '{duplicateEx.TableName}'. Restrição violada: '{duplicateEx.ConstraintName}'");
        }

        var response = CreateUserMapper.FromEntity(entity);
        return Result<CreateUserResponse>.Success(response);
    }
}