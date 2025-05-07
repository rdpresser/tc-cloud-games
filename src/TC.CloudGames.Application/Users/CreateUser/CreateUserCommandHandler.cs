using Ardalis.Result;
using TC.CloudGames.Application.Abstractions.Messaging;
using TC.CloudGames.CrossCutting.Commons.Clock;
using TC.CloudGames.Domain.Abstractions;
using TC.CloudGames.Domain.Exceptions;
using TC.CloudGames.Domain.User;

namespace TC.CloudGames.Application.Users.CreateUser;

internal sealed class
    CreateUserCommandHandler : CommandHandler<CreateUserCommand, CreateUserResponse, User, IUserEfRepository>
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateUserCommandHandler(IUnitOfWork unitOfWork, IUserEfRepository repository,
        IDateTimeProvider dateTimeProvider)
        : base(unitOfWork, repository)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public override async Task<Result<CreateUserResponse>> ExecuteAsync(CreateUserCommand command,
        CancellationToken ct = default)
    {
        var entity = CreateUserMapper.ToEntity(command, _dateTimeProvider);

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