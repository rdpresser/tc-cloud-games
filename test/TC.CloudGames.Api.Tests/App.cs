using Microsoft.AspNetCore.Hosting;

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
            // services.AddSingleton<IMyService, MyMockService>();

            // You can also remove or modify services as needed for tests
        }

        // Runs once after all tests in this fixture
        protected override ValueTask TearDownAsync()
        {
            // Example: Clean up test data, files, or resources
            return ValueTask.CompletedTask;
        }
    }
}
