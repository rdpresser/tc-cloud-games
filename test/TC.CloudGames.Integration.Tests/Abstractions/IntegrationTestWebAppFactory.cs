using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using TC.CloudGames.Application.Users.Login;
using TC.CloudGames.Infra.CrossCutting.Commons.Authentication;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace TC.CloudGames.Integration.Tests.Abstractions
{
    public class IntegrationTestWebAppFactory : WebApplicationFactory<Api.Program>, IAsyncLifetime
    {
        internal IConfiguration Configuration { get; private set; } = default!;
        internal JwtSettings JwtAppSettings { get; private set; } = default!;

        internal HttpClient GuestClient { get; private set; } = default!;
        internal HttpClient AdminClient { get; private set; } = default!;
        internal HttpClient UserClient { get; private set; } = default!;

        private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithDatabase("tc_cloud_games")
            .WithPortBinding(5432, true) // Bind to a random port on the host
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        private readonly RedisContainer _redisContainer = new RedisBuilder()
            .WithImage("redis:latest")
            .WithPortBinding(6379, true) // Bind to a random port on the host
            .Build();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
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

            builder.ConfigureTestServices(services =>
            {

            });
        }

        public async Task SetupAsync()
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            JwtAppSettings = Configuration.GetSection("Jwt").Get<JwtSettings>()
                ?? throw new InvalidOperationException("JwtSettings configuration section is missing or invalid.");

            // Create a guest user for testing purposes
            GuestClient = CreateClient();

            // Simulate a regular user login to get a JWT token
            await DoLogin(UserClient, email: "user@user.com", password: "User@123");

            // Simulate an admin user login to get a JWT token            
            await DoLogin(AdminClient, email: "admin@admin.com", password: "Admin@123");
        }

        public async Task<(HttpResponseMessage, LoginUserResponse)> DoLogin(HttpClient httpClient, string email, string password)
        {
            httpClient = httpClient ?? CreateClient();
            var loginUserReq = new LoginUserCommand(Email: email, Password: password);
            var result = await httpClient.PostAsJsonAsync("auth/login", loginUserReq).ConfigureAwait(false);

            result.EnsureSuccessStatusCode(); // Ensure the request was successful

            var loginResponse = await result.Content.ReadFromJsonAsync<LoginUserResponse>().ConfigureAwait(false);
            var jwtToken = loginResponse?.JwtToken ?? throw new InvalidOperationException("Login response did not contain a JWT token.");
            httpClient.DefaultRequestHeaders.Authorization = new("Bearer", jwtToken);

            return (result, loginResponse);
        }

        public async ValueTask InitializeAsync()
        {
            await _dbContainer.StartAsync();
            await _redisContainer.StartAsync();
            await SetupAsync();
        }

        public override async ValueTask DisposeAsync()
        {
            await _dbContainer.StopAsync();
            await _dbContainer.DisposeAsync();

            await _redisContainer.StopAsync();
            await _redisContainer.DisposeAsync();

            GC.SuppressFinalize(this);
        }
    }
}
