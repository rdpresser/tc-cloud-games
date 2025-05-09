using Ardalis.Result;
using TC.CloudGames.Application.Abstractions.Data;
using TC.CloudGames.Application.Abstractions.Messaging;
using TC.CloudGames.Domain.Exceptions;
using TC.CloudGames.Domain.User;
using TC.CloudGames.Infra.CrossCutting.Commons.Authentication;
using TC.CloudGames.Infra.CrossCutting.Commons.Clock;

namespace TC.CloudGames.Application.Users.CreateUser;

internal sealed class
    CreateUserCommandHandler : CommandHandler<CreateUserCommand, CreateUserResponse, User, IUserEfRepository>
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IPasswordHasher _passwordHasher;

    public CreateUserCommandHandler(IUnitOfWork unitOfWork, IUserEfRepository repository,
        IDateTimeProvider dateTimeProvider, IPasswordHasher passwordHasher)
        : base(unitOfWork, repository)
    {
        _dateTimeProvider = dateTimeProvider;
        _passwordHasher = passwordHasher;
    }

    public override async Task<Result<CreateUserResponse>> ExecuteAsync(CreateUserCommand command,
        CancellationToken ct = default)
    {
        var entity = CreateUserMapper.ToEntity(command, _dateTimeProvider, _passwordHasher);

        try
        {
            // Check if the email already exists
            if (await Repository.EmailExistsAsync(command.Email, ct).ConfigureAwait(false))
            {
                AddError(x => x.Email, UserDomainErrors.EmailAlreadyExists.ErrorMessage,
                    UserDomainErrors.EmailAlreadyExists.ErrorCode);
                return ValidationErrorsInvalid();
            }

            Repository.Add(entity);

            await UnitOfWork.SaveChangesAsync(ct).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is IDuplicateKeyException duplicateEx)
        {
            return HandleDuplicateKeyException(duplicateEx);
        }

        return CreateUserMapper.FromEntity(entity);
    }
}