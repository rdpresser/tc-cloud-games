using HealthChecks.UI.Client;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using TC.CloudGames.Api.Middleware;
using TC.CloudGames.Infra.CrossCutting.Commons.Extensions;

namespace TC.CloudGames.Api.Extensions;

[ExcludeFromCodeCoverage]
internal static class ApplicationBuilderExtensions
{
    // Applies pending migrations to the database
    public static async Task ApplyMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await dbContext.Database.MigrateAsync().ConfigureAwait(false);
    }

    // Configures the custom exception handling middleware
    private static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        return app;
    }

    // Configures FastEndpoints with custom settings and Swagger generation
    public static IApplicationBuilder UseCustomFastEndpoints(this IApplicationBuilder app)
    {
        app.UseFastEndpoints(c =>
            {
                c.Endpoints.RoutePrefix = "api";
                c.Endpoints.ShortNames = true;
                c.Errors.ProducesMetadataType = typeof(Microsoft.AspNetCore.Mvc.ProblemDetails);
                c.Errors.ResponseBuilder = (failures, ctx, statusCode) =>
                {
                    var errors = failures.Select(f => new
                    {
                        name = f.PropertyName.ToPascalCaseFirst(),
                        reason = f.ErrorMessage,
                        code = f.ErrorCode
                    }).ToArray();

                    string title = statusCode switch
                    {
                        400 => "Validation Error",
                        404 => "Not Found",
                        403 => "Forbidden",
                        _ => "One or more errors occurred!"
                    };

                    var problemDetails = new Microsoft.AspNetCore.Mvc.ProblemDetails
                    {
                        Status = statusCode,
                        Instance = ctx.Request.Path.Value ?? string.Empty,
                        Type = "https://www.rfc-editor.org/rfc/rfc7231#section-6.5.1",
                        Title = title,
                    };

                    problemDetails.Extensions["traceId"] = ctx.TraceIdentifier;
                    problemDetails.Extensions["errors"] = errors;

                    return problemDetails;
                };
            })
            .UseSwaggerGen();

        return app;
    }

    // Configures custom middlewares including HTTPS redirection, exception handling, correlation, logging, and health checks
    public static IApplicationBuilder UseCustomMiddlewares(this IApplicationBuilder app)
    {
        app.UseHttpsRedirection()
            .UseCustomExceptionHandler()
            .UseCorrelationMiddleware()
            .UseMiddleware<TelemetryMiddleware>() // Add telemetry middleware after correlation
            .UseSerilogRequestLogging()
            .UseHealthChecks("/health", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            })
            // Add Prometheus metrics endpoint
            .UseOpenTelemetryPrometheusScrapingEndpoint("/metrics");

        return app;
    }
}