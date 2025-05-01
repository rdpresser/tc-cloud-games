using Ardalis.Result;
using TC.CloudGames.Application.Abstractions.Messaging;
using TC.CloudGames.Domain.Abstractions;
using TC.CloudGames.Domain.Exceptions;
using TC.CloudGames.Domain.User;

namespace TC.CloudGames.Application.Users.CreateUser;

internal sealed class CreateUserCommandHandler : CommandHandler<CreateUserCommand, CreateUserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public override async Task<Result<CreateUserResponse>> ExecuteAsync(CreateUserCommand command, CancellationToken ct)
    {
        var entity = CreateUserMapper.ToEntity(command);

        try
        {
            // Check if the email already exists
            if (await _userRepository.EmailExistsAsync(command.Email, ct).ConfigureAwait(false))
            {
                AddError(x => x.Email, UserDomainErrors.EmailAlreadyExists.ErrorMessage,
                    UserDomainErrors.EmailAlreadyExists.ErrorCode);
                return ValidationErrorsInvalid();
            }

            _userRepository.Add(entity);

            await _unitOfWork.SaveChangesAsync(ct).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is IDuplicateKeyException duplicateEx)
        {
            return HandleDuplicateKeyException(duplicateEx);
        }

        return CreateUserMapper.FromEntity(entity);
    }
}