using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using TC.CloudGames.Api.Endpoints.Auth;
using TC.CloudGames.Application.Users.Login;
using TC.CloudGames.Infra.CrossCutting.Commons.Authentication;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace TC.CloudGames.Integration.Tests.Abstractions
{
    public class App : AppFixture<Api.Program>
    {
        internal IConfiguration Configuration { get; private set; } = default!;
        internal JwtSettings JwtAppSettings { get; private set; } = default!;
        internal HttpClient GuestClient { get; private set; } = default!;
        internal HttpClient AdminClient { get; private set; } = default!;
        internal HttpClient UserClient { get; private set; } = default!;

        private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
                    .WithName("tc-cloudgames-db-tests")
                    .WithImage("postgres:latest")
                    .WithDatabase("tc_cloud_games")
                    .WithUsername("postgres")
                    .WithPortBinding(5432, true) // Bind to a random port on the host
                    .WithPassword("postgres")
                    .Build();

        private readonly RedisContainer _redisContainer = new RedisBuilder()
            .WithImage("redis:latest")
            .WithPortBinding(6379, true) // Bind to a random port on the host
            .WithName("tc-cloudgames-redis-tests")
            .Build();

        // Runs once before any tests in this fixture
        protected override async ValueTask SetupAsync()
        {
            // Create a new client for the test server without authentication (guest client)
            GuestClient = CreateClient();

            // Create a new client for the test server
            UserClient = CreateClient();
            var loginUserReq = new LoginUserCommand(Email: "user@user.com", Password: "User@123");
            var (_, resultUser) = await UserClient.POSTAsync<LoginEndpoint, LoginUserCommand, LoginUserResponse>(loginUserReq);
            UserClient.DefaultRequestHeaders.Authorization = new("Bearer", resultUser?.JwtToken);

            // Create a new client for the test server with admin privileges
            AdminClient = CreateClient();
            var loginAdminReq = new LoginUserCommand(Email: "admin@admin.com", Password: "Admin@123");
            var (_, resultAdmin) = await AdminClient.POSTAsync<LoginEndpoint, LoginUserCommand, LoginUserResponse>(loginAdminReq);
            AdminClient.DefaultRequestHeaders.Authorization = new("Bearer", resultAdmin?.JwtToken);
        }

        // Runs once after all tests in this fixture
        protected override async ValueTask PreSetupAsync()
        {
            await _dbContainer.StartAsync();
            await _redisContainer.StartAsync();
        }

        // Configure the web host builder before the app starts
        protected override void ConfigureApp(IWebHostBuilder a)
        {
            a.UseEnvironment("Development");
            a.UseContentRoot(Directory.GetCurrentDirectory());

            a.ConfigureAppConfiguration((context, config) =>
            {
                // Build the configuration and retrieve the IConfiguration instance
                Configuration = config.Build();
                JwtAppSettings = Configuration.GetSection("Jwt").Get<JwtSettings>() ??
                    throw new InvalidOperationException("JwtSettings configuration section is missing or invalid.");
            });

            var tokens = ConnectionStringParser.Parse(_dbContainer.GetConnectionString());
            Environment.SetEnvironmentVariable("Database:Host", tokens["Host"]);
            Environment.SetEnvironmentVariable("Database:Port", tokens["Port"]);
            Environment.SetEnvironmentVariable("Database:Name", tokens["Database"]);
            Environment.SetEnvironmentVariable("Database:User", tokens["Username"]);
            Environment.SetEnvironmentVariable("Database:Password", tokens["Password"]);
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");

            var cacheTokens = CacheStringParser.Parse(_redisContainer.GetConnectionString());
            Environment.SetEnvironmentVariable("Cache:Host", cacheTokens["Host"]);
            Environment.SetEnvironmentVariable("Cache:Port", cacheTokens["Port"]);
            Environment.SetEnvironmentVariable("Cache:Password", cacheTokens.TryGetValue("Password", out var password) ? password : string.Empty);
            Environment.SetEnvironmentVariable("Cache:InstanceName", cacheTokens.TryGetValue("InstanceName", out var instanceName) ? instanceName : "API.IntegrationTests:");
        }

        // Register or override services for testing
        protected override void ConfigureServices(IServiceCollection s)
        {

        }

        // Runs once after all tests in this fixture
        protected override async ValueTask TearDownAsync()
        {
            await _dbContainer.StopAsync();
            await _dbContainer.DisposeAsync();

            await _redisContainer.StopAsync();
            await _redisContainer.DisposeAsync();

            AdminClient.Dispose();
            UserClient.Dispose();
            GuestClient.Dispose();
        }
    }
}
