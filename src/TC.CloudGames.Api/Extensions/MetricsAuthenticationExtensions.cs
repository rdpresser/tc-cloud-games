using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace TC.CloudGames.Api.Extensions
{
    public static class MetricsAuthenticationExtensions
    {
        public static IApplicationBuilder UseMetricsAuthentication(this IApplicationBuilder app)
        {
            return app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/metrics")
                {
                    var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                    if (authHeader?.StartsWith("Bearer ") == true)
                    {
                        var token = authHeader.Substring("Bearer ".Length).Trim();
                        var expectedToken = Environment.GetEnvironmentVariable("GRAFANA_PROMETHEUS_TOKEN");

                        if (token == expectedToken)
                        {
                            await next();
                            return;
                        }
                    }

                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Unauthorized");
                    return;
                }

                await next();
            });
        }
    }
}
