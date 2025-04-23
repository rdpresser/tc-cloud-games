using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using TC.CloudGames.Api.Extensions;
using TC.CloudGames.Application.Middleware;
using TC.CloudGames.CrossCutting.Commons.Extensions;
using TC.CloudGames.CrossCutting.Commons.Middleware;
using TC.CloudGames.CrossCutting.IoC;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAuthenticationJwtBearer(s => s.SigningKey = builder.Configuration["JwtSecretKey"])
    .AddAuthorization()
    .AddFastEndpoints(dicoveryOptions =>
    {
        dicoveryOptions.Assemblies = [typeof(TC.CloudGames.Application.Abstractions.Messaging.ICommand<>).Assembly];
    })
    .SwaggerDocument();

builder.Services
    .AddDependencyInjection(builder.Configuration)
    .AddCorrelationIdGenerator()
    .AddHttpClient();

builder.Services.AddCommandMiddleware(
    c =>
    {
        c.Register(
            typeof(CommandLogger<,>),
            typeof(CommandValidator<,>));
    });

var app = builder.Build();

app.UseAuthentication()
    .UseAuthorization()
    .UseFastEndpoints(c =>
    {
        c.Errors.UseProblemDetails(
            x =>
            {
                x.AllowDuplicateErrors = true;  //allows duplicate errors for the same error name
                x.IndicateErrorCode = false;     //serializes the fluentvalidation error code
                x.IndicateErrorSeverity = false; //serializes the fluentvalidation error severity
                x.TypeValue = "https://www.rfc-editor.org/rfc/rfc7231#section-6.5.1";
                x.TitleValue = "One or more validation errors occurred.";
                x.TitleTransformer = pd => pd.Status switch
                {
                    400 => "Validation Error",
                    404 => "Not Found",
                    _ => "One or more errors occurred!"
                };
            });
        c.Errors.ProducesMetadataType = typeof(ProblemDetails);
    })
    .UseSwaggerGen();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.ApplyMigrations();

    //await app.SeedData()
    //    .ConfigureAwait(false);
}

app.UseHttpsRedirection();

app.UseCustomExceptionHandler()
   .UseCorrelationMiddleware();

app.Run();