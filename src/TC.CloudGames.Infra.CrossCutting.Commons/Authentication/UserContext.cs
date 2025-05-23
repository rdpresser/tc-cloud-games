using Microsoft.AspNetCore.Http;

namespace TC.CloudGames.Infra.CrossCutting.Commons.Authentication
{
    public sealed class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid UserId =>
            _httpContextAccessor
                .HttpContext?
                .User
                .GetUserId() ??
            throw new InvalidOperationException("User context is unavailable");


        public string UserEmail =>
            _httpContextAccessor
                .HttpContext?
                .User
                .GetUserEmail() ??
            throw new InvalidOperationException("User context is unavailable");

        public string UserName =>
            _httpContextAccessor
                .HttpContext?
                .User
                .GetUserName() ??
            throw new InvalidOperationException("User context is unavailable");

        public string UserRole =>
            _httpContextAccessor
                .HttpContext?
                .User
                .GetUserRole() ??
            throw new InvalidOperationException("User context is unavailable");
    }
}
