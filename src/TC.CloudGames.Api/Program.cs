using TC.CloudGames.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
   .AddCustomAuthentication(builder.Configuration)
   .AddCustomFastEndpoints()
   .AddCustomServices(builder.Configuration)
   .AddCustomMiddleware()
   .ConfigureDatabaseSettings(builder.Configuration);

var app = builder.Build();

app.UseAuthentication()
  .UseAuthorization()
  .UseCustomFastEndpoints()
  .UseCustomMiddlewares();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    await app.ApplyMigrations().ConfigureAwait(false);
}

await app.RunAsync();