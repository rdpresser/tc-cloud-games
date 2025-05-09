using FastEndpoints.Security;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace TC.CloudGames.Infra.CrossCutting.Commons.Authentication
{
    public sealed class TokenProvider(IOptions<JwtSettings> jwtSettings) : ITokenProvider
    {
        private readonly JwtSettings _jwtSettings = jwtSettings.Value;

        public string Create(UserTokenProvider user)
        {
            var token = JwtBearer.CreateToken(options =>
            {
                options.SigningKey = _jwtSettings.SecretKey;
                options.User.Claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()));
                options.User.Claims.Add(new Claim(JwtRegisteredClaimNames.Name, $"{user.FirstName} {user.LastName}"));
                options.User.Claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
                options.User.Roles.Add(user.Role);
                options.ExpireAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes);
                options.Issuer = _jwtSettings.Issuer;
                options.Audience = _jwtSettings.Audience;
            });

            return token;
        }
    }
}
