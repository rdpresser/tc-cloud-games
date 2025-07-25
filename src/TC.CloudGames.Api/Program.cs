using TC.CloudGames.Api.Telemetry;
using ServiceCollectionExtensions = TC.CloudGames.Api.Extensions.ServiceCollectionExtensions;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load(Path.Combine("./", ".env"));

builder.Host.UseCustomSerilog(builder.Configuration);

ServiceCollectionExtensions.ConfigureFluentValidationGlobals();

builder.AddCustomLoggingTelemetry();

builder.Services
   .AddCustomServices(builder.Configuration)
   .AddCustomOpenTelemetry(builder.Configuration)
   .AddCustomAuthentication(builder.Configuration)
   .AddCustomFastEndpoints()
   //.AddCustomMiddleware()
   .ConfigureAppSettings(builder.Configuration)
   .AddCustomHealthCheck();

var app = builder.Build();

// Get logger instance for Program and log telemetry configuration
var logger = app.Services.GetRequiredService<ILogger<TC.CloudGames.Api.Program>>();
TelemetryConstants.LogTelemetryConfiguration(logger);

// Use metrics authentication middleware extension
app.UseMetricsAuthentication();

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