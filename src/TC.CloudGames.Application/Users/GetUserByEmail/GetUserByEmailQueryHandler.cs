using Ardalis.Result;
using TC.CloudGames.Application.Abstractions;
using TC.CloudGames.Application.Abstractions.Data;
using TC.CloudGames.Application.Abstractions.Messaging;
using TC.CloudGames.Application.Users.GetUser;
using TC.CloudGames.Domain.User.Abstractions;
using TC.CloudGames.Infra.CrossCutting.Commons.Authentication;

namespace TC.CloudGames.Application.Users.GetUserByEmail
{
    internal sealed class GetUserByEmailQueryHandler : QueryHandler<GetUserByEmailQuery, UserByEmailResponse>
    {
        private readonly IUserPgRepository _userRepository;
        private readonly IUserContext _userContext;

        public GetUserByEmailQueryHandler(IUserPgRepository userRepository, IUserContext userContext)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
        }

        public override async Task<Result<UserByEmailResponse>> ExecuteAsync(GetUserByEmailQuery command, CancellationToken ct = default)
        {
            UserByEmailResponse? userResponse = null;

            if (_userContext.UserRole == AppConstants.UserRole
                && !_userContext.UserEmail.Equals(command.Email, StringComparison.InvariantCultureIgnoreCase))
            {
                AddError(x => x.Email, "You are not authorized to access this user.", $"{nameof(GetUserByEmailQuery.Email)}.NotAuthorized");
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