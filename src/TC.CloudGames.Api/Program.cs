using FastEndpoints;
using FastEndpoints.Swagger;
using TC.CloudGames.Application.Users.CreateUser;
using TC.CloudGames.Domain.CrossCutting.IoC;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddFastEndpoints()
    .SwaggerDocument();

builder.Services
    .AddDependencyInjection(builder.Configuration)
    .AddHttpClient();

builder.Services
    .AddScoped<Validator<CreateUserCommand>, CreateUserRequestValidator>();

var app = builder.Build();

//app.UseAuthentication();
//app.UseAuthorization();

app.UseFastEndpoints()
    .UseSwaggerGen();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();