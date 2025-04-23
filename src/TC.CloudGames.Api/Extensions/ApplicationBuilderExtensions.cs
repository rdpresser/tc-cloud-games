using Microsoft.EntityFrameworkCore;
using TC.CloudGames.Api.Middleware;
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
    }
}
