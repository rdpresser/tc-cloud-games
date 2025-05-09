using Ardalis.Result;
using TC.CloudGames.Application.Abstractions.Data;
using TC.CloudGames.Application.Abstractions.Messaging;
using TC.CloudGames.Domain.User;
using TC.CloudGames.Infra.CrossCutting.Commons.Authentication;

namespace TC.CloudGames.Application.Users.Login;

internal sealed class LoginUserCommandHandler : CommandHandler<LoginUserCommand, LoginUserResponse, User, IUserEfRepository>
{
    private readonly IUserEfRepository _userRepository;
    private readonly ITokenProvider _tokenProvider;
    private readonly IPasswordHasher _passwordHasher;

    public LoginUserCommandHandler(IUnitOfWork unitOfWork, IUserEfRepository userRepository, ITokenProvider tokenProvider, IPasswordHasher passwordHasher)
        : base(unitOfWork, userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _tokenProvider = tokenProvider;
        _passwordHasher = passwordHasher;
    }

    public override async Task<Result<LoginUserResponse>> ExecuteAsync(LoginUserCommand command, CancellationToken ct)
    {
        var userDb = await _userRepository
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