using TC.CloudGames.Application.Abstractions.Data;
using TC.CloudGames.Domain.Exceptions;
using TC.CloudGames.Domain.UserAggregate;
using TC.CloudGames.Domain.UserAggregate.Abstractions;

namespace TC.CloudGames.Application.Users.CreateUser;

internal sealed class CreateUserCommandHandler : CommandHandler<CreateUserCommand, CreateUserResponse, User, IUserEfRepository>
{
    public CreateUserCommandHandler(IUnitOfWork unitOfWork, IUserEfRepository repository)
        : base(unitOfWork, repository)
    {

    }

    public override async Task<Result<CreateUserResponse>> ExecuteAsync(CreateUserCommand command,
        CancellationToken ct = default)
    {
        var entity = await CreateUserMapper.ToEntityAsync(command, Repository).ConfigureAwait(false);

        if (!entity.IsSuccess)
        {
            AddErrors(entity.ValidationErrors);
            return Result.Invalid(entity.ValidationErrors);
        }

        try
        {
            Repository.Add(entity);

            await UnitOfWork.SaveChangesAsync(ct).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is IDuplicateKeyViolation duplicateEx)
        {
            return HandleDuplicateKeyException(duplicateEx);
        }

        return CreateUserMapper.FromEntity(entity);
    }
}