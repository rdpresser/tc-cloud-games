using TC.CloudGames.Application.Abstractions.Data;
using TC.CloudGames.Domain.User;
using TC.CloudGames.Domain.User.Abstractions;
using TC.CloudGames.Infra.CrossCutting.Commons.Authentication;

namespace TC.CloudGames.Application.Users.Login;

internal sealed class LoginUserCommandHandler : CommandHandler<LoginUserCommand, LoginUserResponse, User, IUserEfRepository>
{
    private readonly ITokenProvider _tokenProvider;

    public LoginUserCommandHandler(IUnitOfWork unitOfWork, IUserEfRepository userRepository, ITokenProvider tokenProvider)
        : base(unitOfWork, userRepository)
    {
        _tokenProvider = tokenProvider;
    }

    public override async Task<Result<LoginUserResponse>> ExecuteAsync(LoginUserCommand command, CancellationToken ct = default)
    {
        var userDb = await Repository
            .GetByEmailWithPasswordAsync
            (
                command.Email,
                command.Password,
                ct
            ).ConfigureAwait(false);

        if (userDb is null)
        {
            AddError(UserDomainErrors.InvalidCredentials.Property, UserDomainErrors.InvalidCredentials.ErrorMessage,
                UserDomainErrors.InvalidCredentials.ErrorCode);
            return ValidationErrorNotFound();
        }

        return new LoginUserResponse(
            JwtToken: _tokenProvider.Create(new(userDb.Id, userDb.FirstName, userDb.LastName, userDb.Email.ToString(), userDb.Role.ToString())),
            Email: userDb.Email.Value
        );
    }
}