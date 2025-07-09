using ServiceCollectionExtensions = TC.CloudGames.Api.Extensions.ServiceCollectionExtensions;

var builder = WebApplication.CreateBuilder(args);

// TODO: Make this conditional based on environment
builder.Host.UseCustomSerilog(builder.Configuration);

ServiceCollectionExtensions.ConfigureFluentValidationGlobals();

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


Log.Information("Teste de log para Grafana Loki");
await app.RunAsync().ConfigureAwait(false);