using Serilog.Enrichers.Sensitive;
using Serilog.Enrichers.Span;
using Serilog.Sinks.Grafana.Loki;
using System.Diagnostics.CodeAnalysis;
using TC.CloudGames.Api.Telemetry;

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

                // Get consistent values using centralized constants
                var environment = configuration["ASPNETCORE_ENVIRONMENT"]?.ToLower() ?? "development";
                var serviceVersion = typeof(Program).Assembly.GetName().Version?.ToString() ?? TelemetryConstants.Version;

                // Timezone customizado
                var timeZoneId = configuration["TimeZone"] ?? "UTC";
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                loggerConfiguration.Enrich.With(new UtcToLocalTimeEnricher(timeZone));

                // Enrich com trace_id e span_id
                loggerConfiguration.Enrich.WithSpan();

                // Use OpenTelemetry semantic conventions (dot notation) for Serilog properties
                loggerConfiguration.Enrich.WithProperty("service.name", TelemetryConstants.ServiceName);
                loggerConfiguration.Enrich.WithProperty("service.namespace", TelemetryConstants.ServiceNamespace);
                loggerConfiguration.Enrich.WithProperty("service.version", serviceVersion);
                loggerConfiguration.Enrich.WithProperty("deployment.environment", environment);
                
                // Additional OpenTelemetry resource attributes for consistency
                loggerConfiguration.Enrich.WithProperty("cloud.provider", "azure");
                loggerConfiguration.Enrich.WithProperty("cloud.platform", "azure_container_apps");
                loggerConfiguration.Enrich.WithProperty("service.instance.id", Environment.MachineName);

                // Sensitive data masking
                loggerConfiguration.Enrich.WithSensitiveDataMasking(options =>
                {
                    options.MaskProperties = ["Password", "Email", "PhoneNumber"];
                });

                // Console sink for local/dev visibility
                loggerConfiguration.WriteTo.Console();

                // Loki sink with FIXED label naming (underscores for Grafana Cloud compatibility)
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
                            // CRITICAL: Use underscores for Loki label compatibility (not dots!)
                            new LokiLabel { Key = "service_name", Value = TelemetryConstants.ServiceName },
                            new LokiLabel { Key = "service_namespace", Value = TelemetryConstants.ServiceNamespace },
                            new LokiLabel { Key = "deployment_environment", Value = environment },
                            new LokiLabel { Key = "cloud_provider", Value = "azure" },
                            new LokiLabel { Key = "service_version", Value = serviceVersion }
                        }
                    );
                }
            });
        }
    }
}