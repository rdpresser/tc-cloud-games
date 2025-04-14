using Microsoft.Extensions.Logging;
using TC.CloudGames.Domain.User;

namespace TC.CloudGames.Application.Users.CreateUser
{
    public class CreateUserApp(ILogger<CreateUserApp> logger, IUserRepository userRepository) : ICreateUserApp
    {
        private readonly ILogger<CreateUserApp> _logger = logger;
        private readonly IUserRepository _userRepository = userRepository;

        public Task<CreateUserResponse> InvokeAsync(CreateUserRequest request, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
