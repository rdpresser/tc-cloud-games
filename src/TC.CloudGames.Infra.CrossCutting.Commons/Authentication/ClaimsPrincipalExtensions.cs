using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace TC.CloudGames.Infra.CrossCutting.Commons.Authentication
{
    internal static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal? principal)
        {
            string? userId = principal?.FindFirstValue(JwtRegisteredClaimNames.Sub);

            return Guid.TryParse(userId, out Guid parsedUserId) ?
                parsedUserId :
                throw new InvalidOperationException("User id is unavailable");
        }

        public static string GetUserEmail(this ClaimsPrincipal? principal)
        {
            string? userEmail = principal?.FindFirstValue(JwtRegisteredClaimNames.Email);
            return string.IsNullOrEmpty(userEmail) ?
                throw new InvalidOperationException("User email is unavailable") :
                userEmail;
        }

        public static string GetUserName(this ClaimsPrincipal? principal)
        {
            string? userName = principal?.FindFirstValue(JwtRegisteredClaimNames.Name);
            return string.IsNullOrEmpty(userName) ?
                throw new InvalidOperationException("User name is unavailable") :
                userName;
        }

        public static string GetUserRole(this ClaimsPrincipal? principal)
        {
            string? userRole = principal?.FindFirstValue("role");
            return string.IsNullOrEmpty(userRole) ?
                throw new InvalidOperationException("User role is unavailable") :
                userRole;
        }
    }
}
