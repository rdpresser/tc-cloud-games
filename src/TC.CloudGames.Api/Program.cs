using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using Newtonsoft.Json.Converters;
using TC.CloudGames.Api.Extensions;
using TC.CloudGames.Application.Middleware;
using TC.CloudGames.CrossCutting.Commons.Extensions;
using TC.CloudGames.CrossCutting.Commons.Middleware;
using TC.CloudGames.CrossCutting.IoC;
using TC.CloudGames.Infra.Data.Configurations.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAuthenticationJwtBearer(s => s.SigningKey = builder.Configuration["JwtSecretKey"])
    .AddAuthorization()
    .AddFastEndpoints(dicoveryOptions =>
    {
        dicoveryOptions.Assemblies = [typeof(TC.CloudGames.Application.Abstractions.Messaging.ICommand<>).Assembly];
    })
    .SwaggerDocument(o =>
    {
        o.DocumentSettings = s =>
        {
            s.Title = "TC.CloudGames API";
            s.Version = "v1";
            s.Description = "API for TC.CloudGames";
            s.MarkNonNullablePropsAsRequired();
        };

        o.RemoveEmptyRequestSchema = true;
        o.NewtonsoftSettings = s =>
        {
            s.Converters.Add(new StringEnumConverter());
        };
    });

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

builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("Database"));

var app = builder.Build();

app.UseAuthentication()
    .UseAuthorization()
    .UseFastEndpoints(c =>
    {
        c.Endpoints.RoutePrefix = "api";
        c.Endpoints.ShortNames = true;
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
    await app.ApplyMigrations().ConfigureAwait(false);

    //await app.SeedData().ConfigureAwait(false);
}

app.UseHttpsRedirection();

app.UseCustomExceptionHandler()
   .UseCorrelationMiddleware();

app.Run();