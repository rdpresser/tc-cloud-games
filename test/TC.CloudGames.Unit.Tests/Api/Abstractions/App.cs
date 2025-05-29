using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;
using TC.CloudGames.Application.Abstractions;
using TC.CloudGames.Infra.CrossCutting.Commons.Authentication;
using TC.CloudGames.Infra.Data;
using ZiggyCreatures.Caching.Fusion;
using DomainGame = TC.CloudGames.Domain.GameAggregate.Game;
using DomainGameDetails = TC.CloudGames.Domain.GameAggregate.ValueObjects.GameDetails;

namespace TC.CloudGames.Unit.Tests.Api.Abstractions
{
    public class App : AppFixture<CloudGames.Api.Program>
    {
        internal readonly List<string> Genres;
        internal readonly List<string> Platforms;
        internal readonly List<string> GameTags;
        internal readonly List<string> GameModes;
        internal readonly List<string> DistributionFormats;
        internal readonly List<string> Languages;
        internal readonly List<string> GameStatus;
        internal readonly List<string> AgeRatings;

        public App()
        {
            Genres = new List<string> { "Action", "Adventure", "RPG", "Strategy", "Simulation", "Racing", "Sport", "Puzzle",
            "Fighter", "Platform", "FPS", "TPS", "Survival", "Horror", "Stealth", "Open World", "MMORPG", "Roguelike",
            "Visual Novel", "Beat 'em up", "Battle Royale", "Musical", "Party Game", "Metroidvania", "Idle/Incremental",
            "Tower Defense", "MOBA", "Sandbox", "Tycoon" };

            Platforms = [.. DomainGameDetails.ValidPlatforms];

            GameTags = new List<string> { "Indie", "Multiplayer", "Singleplayer", "Co-op", "PvP", "PvE", "Online Co-op",
            "Local Multiplayer", "Story Rich", "Difficult", "Casual", "Anime", "Pixel Graphics", "Retro", "Funny", "Atmospheric",
            "Horror", "Sci-fi", "Fantasy", "Cyberpunk", "Steampunk", "Post-apocalyptic", "Choices Matter", "Narration",
            "Character Customization", "Exploration", "Loot", "Crafting", "Building", "Resource Management", "Base Building",
            "Turn-Based", "Real Time", "Fast-Paced", "Third Person", "First Person", "Top-Down", "Isometric", "Stylized",
            "Realistic", "Female Protagonist", "Controller Support", "VR Support", "Moddable", "Replay Value", "Open World",
            "Procedural Generation", "Sandbox", "Nonlinear", "Mystery", "Psychological", "Dark", "Gore", "Violent" };

            GameModes = [.. DomainGameDetails.ValidGameModes];

            DistributionFormats = [.. DomainGameDetails.ValidDistributionFormats];

            Languages = new List<string> { "PT-BR", "EN-US", "ES-ES", "FR-FR", "ZH-CN", "JA-JP", "RU-RU", "KO-KR" };

            GameStatus = [.. DomainGame.ValidGameStatus];

            AgeRatings = [.. AgeRating.ValidRatings];
        }

        // Runs once before any tests in this fixture
        protected override ValueTask SetupAsync()
        {
            // Example: Seed test data, set up test files, etc.
            return ValueTask.CompletedTask;
        }

        // Configure the web host builder before the app starts
        protected override void ConfigureApp(IWebHostBuilder a)
        {
            // Example: Use a different environment for testing
            a.UseEnvironment("Testing");
            // You can also configure test-specific settings here
        }

        // Register or override services for testing
        protected override void ConfigureServices(IServiceCollection s)
        {
            // Remove the existing ApplicationDbContext registration
            var descriptor = s.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

            if (descriptor != null)
                s.Remove(descriptor);

            // Register ApplicationDbContext with in-memory provider
            s.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("TestDb"));

            s.AddFusionCache()
                .WithDefaultEntryOptions(options =>
                {
                    options.Duration = TimeSpan.FromSeconds(20);
                    options.DistributedCacheDuration = TimeSpan.FromSeconds(30);
                });

            s.AddKeyedTransient($"{nameof(ValidUserContextAccessor)}.{AppConstants.AdminRole}", (sp, key) =>
            {
                return ValidUserContextAccessor(sp, AppConstants.AdminRole);
            });

            s.AddKeyedTransient($"{nameof(ValidUserContextAccessor)}.{AppConstants.UserRole}", (sp, key) =>
            {
                return ValidUserContextAccessor(sp, AppConstants.UserRole);
            });

            s.AddKeyedTransient($"{nameof(ValidUserContextAccessor)}.{AppConstants.UnknownRole}", (sp, key) =>
            {
                return ValidUserContextAccessor(sp, AppConstants.UnknownRole);
            });

            s.AddKeyedTransient($"{nameof(ValidLoggedUser)}.{AppConstants.AdminRole}", (sp, key) =>
            {
                return ValidLoggedUser(sp, AppConstants.AdminRole);
            });

            s.AddKeyedTransient($"{nameof(ValidLoggedUser)}.{AppConstants.UserRole}", (sp, key) =>
            {
                return ValidLoggedUser(sp, AppConstants.UserRole);
            });

            s.AddKeyedTransient($"{nameof(ValidLoggedUser)}.{AppConstants.UnknownRole}", (sp, key) =>
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

            return httpContextAccessor;
        }

        protected static IReadOnlyList<Claim> GetClaimsType(string userRole = AppConstants.AdminRole)
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
