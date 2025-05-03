using TC.CloudGames.Api.Extensions;
using TC.CloudGames.Api.Extensions.TC.CloudGames.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseCustomSerilog(builder.Configuration);

builder.Services
   .AddCustomAuthentication(builder.Configuration)
   .AddCustomFastEndpoints()
   .AddCustomServices(builder.Configuration)
   //.AddCustomMiddleware()
   .ConfigureDatabaseSettings(builder.Configuration)
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

await app.RunAsync();