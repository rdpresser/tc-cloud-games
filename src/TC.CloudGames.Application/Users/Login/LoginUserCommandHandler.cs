using Ardalis.Result;
using FastEndpoints.Security;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TC.CloudGames.Application.Abstractions.Messaging;
using TC.CloudGames.Domain.Abstractions;
using TC.CloudGames.Domain.User;

namespace TC.CloudGames.Application.Users.Login;

internal sealed class LoginUserCommandHandler : CommandHandler<LoginUserCommand, LoginUserResponse, User, IUserEfRepository>
{
    private readonly IConfiguration _configuration;
    private readonly IUserEfRepository _userRepository;

    public LoginUserCommandHandler(IUnitOfWork unitOfWork, IUserEfRepository userRepository, IConfiguration configuration)
        : base(unitOfWork, userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
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

        var jwtSecretKey = _configuration["JwtSecretKey"];
        if (string.IsNullOrEmpty(jwtSecretKey))
        {
            AddError(UserDomainErrors.JwtSecretKeyNotConfigured.Property,
                UserDomainErrors.JwtSecretKeyNotConfigured.ErrorMessage,
                UserDomainErrors.JwtSecretKeyNotConfigured.ErrorCode);
            return ValidationErrorsInvalid();
        }

        var jwt = JwtBearer.CreateToken(options =>
        {
            options.SigningKey = jwtSecretKey;
            options.User.Claims.Add(new Claim(JwtRegisteredClaimNames.Sub, userDb.Id.ToString()));
            options.User.Claims.Add(new Claim(JwtRegisteredClaimNames.Name, $"{userDb.FirstName} {userDb.LastName}"));
            options.User.Claims.Add(new Claim(JwtRegisteredClaimNames.Email, userDb.Email.Value));
            options.User.Roles.Add(userDb.Role.Value);
            options.ExpireAt = DateTime.UtcNow.AddHours(1);
        });

        return new LoginUserResponse(
            jwt,
            userDb.Email.Value
        );
    }
}