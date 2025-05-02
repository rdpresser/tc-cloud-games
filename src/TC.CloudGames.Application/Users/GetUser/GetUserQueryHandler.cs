using Ardalis.Result;
using TC.CloudGames.Application.Abstractions.Messaging;
using TC.CloudGames.Domain.User;

namespace TC.CloudGames.Application.Users.GetUser
{
    internal sealed class GetUserQueryHandler : QueryHandler<GetUserQuery, UserResponse>
    {
        private readonly IUserPgRepository _userRepository;

        public GetUserQueryHandler(IUserPgRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public override async Task<Result<UserResponse>> ExecuteAsync(GetUserQuery command, CancellationToken ct)
        {
            var user = await _userRepository.GetByIdAsync(command.Id, ct).ConfigureAwait(false);

            if (user is not null) 
                return user;
            
            AddError(x => x.Id, $"User with id '{command.Id}' not found.", UserDomainErrors.NotFound.ErrorCode);
            return ValidationErrorNotFound();
        }
    }
}