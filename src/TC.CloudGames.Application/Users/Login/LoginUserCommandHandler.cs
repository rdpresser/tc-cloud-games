using Ardalis.Result;
using TC.CloudGames.Domain.User;

namespace TC.CloudGames.Application.Users.Login
{
    internal sealed class LoginUserCommandHandler : Abstractions.Messaging.ICommandHandler<LoginUserCommand, LoginUserResponse>
    {
        private readonly IUserRepository _userRepository;

        public LoginUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<LoginUserResponse>> ExecuteAsync(LoginUserCommand command, CancellationToken ct)
        {
            var userDb = await _userRepository.GetByEmailWithPasswordAsync(
                command.Email,
                command.Password,
                ct).ConfigureAwait(false);

            if (userDb is null)
            {
                return Result<LoginUserResponse>.NotFound("Email or password provided are invalid.");
            }

            var jwt = "teste";
            var response = new LoginUserResponse(
                jwt,
                userDb.Email.Value
            );

            return Result<LoginUserResponse>.Success(response);
        }
    }
}
