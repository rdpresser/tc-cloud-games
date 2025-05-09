using Ardalis.Result;
using TC.CloudGames.Application.Abstractions.Data;
using TC.CloudGames.Application.Abstractions.Messaging;
using TC.CloudGames.Domain.User;
using TC.CloudGames.Infra.CrossCutting.Commons.Authentication;

namespace TC.CloudGames.Application.Users.GetUser
{
    internal sealed class GetUserByEmailQueryHandler : QueryHandler<GetUserByEmailQuery, UserResponse>
    {
        private readonly IUserPgRepository _userRepository;
        private readonly IUserContext _userContext;
        public const string UserRole = "User";

        public GetUserByEmailQueryHandler(IUserPgRepository userRepository, IUserContext userContext)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
        }

        public override async Task<Result<UserResponse>> ExecuteAsync(GetUserByEmailQuery command, CancellationToken ct)
        {
            UserResponse? userResponse = null;

            if (_userContext.UserRole == UserRole && _userContext.UserEmail != command.Email)
            {
                AddError(x => x.Email, "You are not authorized to access this user.", UserDomainErrors.NotFound.ErrorCode);
                return ValidationErrorNotAuthorized();
            }

            userResponse = await _userRepository
                    .GetByEmailAsync(command.Email, ct)
                    .ConfigureAwait(false);

            if (userResponse is not null)
                return userResponse;

            AddError(x => x.Email, $"User with email '{command.Email}' not found.", UserDomainErrors.NotFound.ErrorCode);
            return ValidationErrorNotFound();
        }
    }
}