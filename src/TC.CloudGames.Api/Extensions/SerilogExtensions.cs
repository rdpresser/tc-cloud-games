using Serilog.Enrichers.Sensitive;
using Serilog.Enrichers.Span;
using Serilog.Sinks.Grafana.Loki;
using System.Diagnostics.CodeAnalysis;

namespace TC.CloudGames.Api.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class SerilogExtensions
    {
        public static IHostBuilder UseCustomSerilog(this IHostBuilder hostBuilder, IConfiguration configuration)
        {
            return hostBuilder.UseSerilog((hostContext, services, loggerConfiguration) =>
            {
                loggerConfiguration.ReadFrom.Configuration(hostContext.Configuration);

                // Timezone customizado
                var timeZoneId = configuration["TimeZone"] ?? "UTC";
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                loggerConfiguration.Enrich.With(new UtcToLocalTimeEnricher(timeZone));

                // Enrich com trace_id e span_id
                loggerConfiguration.Enrich.WithSpan();

                // service.name, service.namespace, deployment.environment
                loggerConfiguration.Enrich.WithProperty("service.name", Environment.GetEnvironmentVariable("OTEL_SERVICE_NAME") ?? "tccloudgames-app");
                loggerConfiguration.Enrich.WithProperty("service.namespace", Environment.GetEnvironmentVariable("OTEL_SERVICE_NAMESPACE") ?? "tccloudgames-app-group");
                loggerConfiguration.Enrich.WithProperty("deployment.environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.ToLower() ?? "development");

                // Sensitive data masking
                loggerConfiguration.Enrich.WithSensitiveDataMasking(options =>
                {
                    options.MaskProperties = ["Password", "Email", "PhoneNumber"];
                });

                // Console sink for local/dev visibility
                loggerConfiguration.WriteTo.Console();

                // Loki sink via appsettings (recommended), but if you want to keep code-based fallback:
                var serilogUsing = configuration.GetSection("Serilog:Using").Get<string[]>() ?? [];
                var useLoki = Array.Exists(serilogUsing, s => s == "Serilog.Sinks.Grafana.Loki");
                if (useLoki)
                {
                    var grafanaLokiUrl = configuration["Serilog:WriteTo:1:Args:uri"];
                    if (string.IsNullOrEmpty(grafanaLokiUrl))
                        throw new ArgumentNullException(nameof(grafanaLokiUrl), "GrafanaLokiUrl configuration is required.");

                    loggerConfiguration.WriteTo.GrafanaLoki(
                        uri: grafanaLokiUrl,
                        credentials: new LokiCredentials
                        {
                            Login = configuration["Serilog:WriteTo:1:Args:credentials:username"],
                            Password = Environment.GetEnvironmentVariable("GRAFANA_API_TOKEN")
                        },
                        labels: new[]
                        {
                            //new LokiLabel { Key = "app", Value = "tccloudgames-app" },
                            new LokiLabel { Key = "service_name", Value = "tccloudgames-app" },
                            new LokiLabel { Key = "service_namespace", Value = "tccloudgames-app-group" },
                            new LokiLabel { Key = "deployment_environment", Value = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.ToLower() }
                        }
                    );
                }
            });
        }
    }
}