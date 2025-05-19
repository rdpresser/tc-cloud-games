using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;
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

            services.AddKeyedScoped(nameof(ValidUserContextAccessor), (sp, key) =>
            {
                return ValidUserContextAccessor(sp);
            });

            services.AddKeyedScoped("ValidLoggedUser", (sp, key) =>
            {
                return ValidLoggedUser(sp);
            });
        }

        internal static IUserContext ValidLoggedUser(IServiceProvider sp)
        {
            var httpContextAccessor = sp.GetRequiredKeyedService<IHttpContextAccessor>(nameof(ValidUserContextAccessor));

            // Ensure the IHttpContextAccessor is not null before passing it to UserContext
            return httpContextAccessor == null
                ? throw new InvalidOperationException("IHttpContextAccessor cannot be null.")
                : (IUserContext)new UserContext(httpContextAccessor);
        }

        internal static IHttpContextAccessor ValidUserContextAccessor(IServiceProvider sp)
        {
            var userId = Guid.NewGuid();
            var claims = new List<Claim>
                    {
                        new(JwtRegisteredClaimNames.Sub, userId.ToString()),
                        new(JwtRegisteredClaimNames.Email, "john.doe@test.com"),
                        new(JwtRegisteredClaimNames.Name, "John Doe"),
                        new("role", "User")
                    };

            var identity = new ClaimsIdentity(claims, "TestAuthType");
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

        // Runs once after all tests in this fixture
        protected override ValueTask TearDownAsync()
        {
            // Example: Clean up test data, files, or resources
            return ValueTask.CompletedTask;
        }
    }
}
