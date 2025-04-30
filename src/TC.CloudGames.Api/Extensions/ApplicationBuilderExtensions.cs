using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TC.CloudGames.Api.Middleware;
using TC.CloudGames.CrossCutting.Commons.Middleware;
using TC.CloudGames.Infra.Data;

namespace TC.CloudGames.Api.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static async Task ApplyMigrations(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();

            using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            await dbContext.Database.MigrateAsync().ConfigureAwait(false);
        }

        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            return app;
        }

        public static IApplicationBuilder UseCustomFastEndpoints(this IApplicationBuilder app)
        {
            app.UseFastEndpoints(c =>
                {
                    c.Endpoints.RoutePrefix = "api";
                    c.Endpoints.ShortNames = true;
                    c.Errors.UseProblemDetails(x =>
                    {
                        x.AllowDuplicateErrors = true;
                        x.IndicateErrorCode = false;
                        x.IndicateErrorSeverity = false;
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

            return app;
        }

        public static IApplicationBuilder UseCustomMiddlewares(this IApplicationBuilder app)
        {
            app.UseHttpsRedirection()
                .UseCustomExceptionHandler()
                .UseCorrelationMiddleware()
                .UseSerilogRequestLogging();

            return app;
        }
    }
}