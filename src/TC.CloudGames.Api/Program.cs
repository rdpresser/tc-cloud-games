using TC.CloudGames.Infra.CrossCutting.Commons.Extensions;
using ServiceCollectionExtensions = TC.CloudGames.Api.Extensions.ServiceCollectionExtensions;

var builder = WebApplication.CreateBuilder(args);

var root = SolutionRootFinder.FindRoot(".solution-root", @"/src", @"/app");
DotNetEnv.Env.Load(Path.Combine(root, ".env"));

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