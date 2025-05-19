using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;
using TC.CloudGames.Application.Abstractions;
using TC.CloudGames.Infra.CrossCutting.Commons.Authentication;
using ZiggyCreatures.Caching.Fusion;

namespace TC.CloudGames.Api.Tests
{
    public class App : AppFixture<Program>
    {
        // Runs once before any tests in this fixture
        protected override ValueTask SetupAsync()
        {
            // Example: Seed test data, set up test files, etc.
            return ValueTask.CompletedTask;
        }

        // Configure the web host builder before the app starts
        protected override void ConfigureApp(IWebHostBuilder builder)
        {
            // Example: Use a different environment for testing
            builder.UseEnvironment("Testing");
            // You can also configure test-specific settings here
        }

        // Register or override services for testing
        protected override void ConfigureServices(IServiceCollection services)
        {
            // Example: Replace a real service with a mock or test double

            services
                .AddFusionCache()
                .WithDefaultEntryOptions(options =>
                {
                    options.Duration = TimeSpan.FromSeconds(20);
                    options.DistributedCacheDuration = TimeSpan.FromSeconds(30);
                });

            services.AddKeyedTransient($"{nameof(ValidUserContextAccessor)}.{AppConstants.AdminRole}", (sp, key) =>
            {
                return ValidUserContextAccessor(sp, AppConstants.AdminRole);
            });

            services.AddKeyedTransient($"{nameof(ValidUserContextAccessor)}.{AppConstants.UserRole}", (sp, key) =>
            {
                return ValidUserContextAccessor(sp, AppConstants.UserRole);
            });

            services.AddKeyedTransient($"{nameof(ValidUserContextAccessor)}.{AppConstants.UnknownRole}", (sp, key) =>
            {
                return ValidUserContextAccessor(sp, AppConstants.UnknownRole);
            });

            services.AddKeyedTransient($"{nameof(ValidLoggedUser)}.{AppConstants.AdminRole}", (sp, key) =>
            {
                return ValidLoggedUser(sp, AppConstants.AdminRole);
            });

            services.AddKeyedTransient($"{nameof(ValidLoggedUser)}.{AppConstants.UserRole}", (sp, key) =>
            {
                return ValidLoggedUser(sp, AppConstants.UserRole);
            });

            services.AddKeyedTransient($"{nameof(ValidLoggedUser)}.{AppConstants.UnknownRole}", (sp, key) =>
            {
                return ValidLoggedUser(sp, AppConstants.UnknownRole);
            });
        }

        protected static IUserContext ValidLoggedUser(IServiceProvider sp, string userRole = AppConstants.AdminRole)
        {
            var httpContextAccessor = sp.GetRequiredKeyedService<IHttpContextAccessor>($"{nameof(ValidUserContextAccessor)}.{userRole}");

            // Ensure the IHttpContextAccessor is not null before passing it to UserContext
            return httpContextAccessor == null
                ? throw new InvalidOperationException("IHttpContextAccessor cannot be null.")
                : (IUserContext)new UserContext(httpContextAccessor);
        }

        protected static IHttpContextAccessor ValidUserContextAccessor(IServiceProvider sp, string userRole = AppConstants.AdminRole)
        {
            var identity = new ClaimsIdentity(GetClaimsType(userRole), "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var httpContextAccessor = new HttpContextAccessor();
            var httpContext = new DefaultHttpContext
            {
                User = claimsPrincipal,
                RequestServices = sp
            };
            httpContextAccessor.HttpContext = httpContext;

            // Ensure the returned IHttpContextAccessor is not null
            return (IHttpContextAccessor)httpContextAccessor ??
                throw new InvalidOperationException("HttpContextAccessor cannot be null.");
        }

        protected static List<Claim> GetClaimsType(string userRole = AppConstants.AdminRole)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(userRole);

            var userId = Guid.NewGuid();
            var claims = new List<Claim>();

            if (userRole == AppConstants.AdminRole)
            {
                claims.AddRange
                (
                    new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, "admin@admin.com"),
                    new Claim(JwtRegisteredClaimNames.Name, "Admin User"),
                    new Claim("role", AppConstants.AdminRole)
                );
            }
            else if (userRole == AppConstants.UserRole)
            {
                claims.AddRange
                (
                    new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, "user@user.com"),
                    new Claim(JwtRegisteredClaimNames.Name, "Regular User"),
                    new Claim("role", AppConstants.UserRole)
                );
            }
            else
            {
                claims.AddRange
                (
                    new Claim(JwtRegisteredClaimNames.Sub, Guid.Empty.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, "fake.doe@test.com"),
                    new Claim(JwtRegisteredClaimNames.Name, ""),
                    new Claim("role", "")
                );
            }

            return claims;
        }

        internal IFusionCache GetCache()
        {
            return Services.GetRequiredService<IFusionCache>();
        }

        internal IUserContext GetValidLoggedUser(string userRole = AppConstants.AdminRole)
        {
            return Services.GetRequiredKeyedService<IUserContext>($"{nameof(ValidLoggedUser)}.{userRole}");
        }

        internal IHttpContextAccessor GetValidUserContextAccessor(string userRole = AppConstants.AdminRole)
        {
            return Services.GetRequiredKeyedService<IHttpContextAccessor>($"{nameof(ValidUserContextAccessor)}.{userRole}");
        }

        // Runs once after all tests in this fixture
        protected override ValueTask TearDownAsync()
        {
            // Example: Clean up test data, files, or resources
            return ValueTask.CompletedTask;
        }
    }
}
