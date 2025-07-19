using ServiceCollectionExtensions = TC.CloudGames.Api.Extensions.ServiceCollectionExtensions;

var builder = WebApplication.CreateBuilder(args);

//string GetEnvRoot()
//{
//    // If running in Docker, likely Linux and /app or /src will exist
//    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
//    {
//        // On Windows, start from current directory
//        return SolutionRootFinder.FindRoot();
//    }
//    else
//    {
//        // On Linux (Docker), try /app and /src
//        return SolutionRootFinder.FindRoot(".solution-root", "/TC.CloudGames.Unit.Tests", "/app", "/src");
//    }
//}

DotNetEnv.Env.Load(Path.Combine("./", ".env"));

// TODO: Make this conditional based on environment
builder.Host.UseCustomSerilog(builder.Configuration);

ServiceCollectionExtensions.ConfigureFluentValidationGlobals();

builder.AddCustomLoggingTelemetry();

builder.Services
   .AddCustomOpenTelemetry(builder.Configuration)
   .AddCustomAuthentication(builder.Configuration)
   .AddCustomFastEndpoints()
   .AddCustomServices(builder.Configuration)
   //.AddCustomMiddleware()
   .ConfigureAppSettings(builder.Configuration)
   .AddCustomHealthCheck();

var app = builder.Build();

app.UseAuthentication()
  .UseAuthorization()
  .UseCustomFastEndpoints()
  .UseCustomMiddlewares();

if (app.Environment.IsDevelopment())
{
    await app.ApplyMigrations().ConfigureAwait(false);
    await app.SeedUserData().ConfigureAwait(false);
    await app.SeedGameData().ConfigureAwait(false);
}

Serilog.Debugging.SelfLog.Enable(Console.Error);
await app.RunAsync().ConfigureAwait(false);