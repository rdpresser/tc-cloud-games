using System.Diagnostics.CodeAnalysis;
using Serilog.Enrichers.Sensitive;
using Serilog.Sinks.Grafana.Loki;

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

                // Sensitive data masking
                loggerConfiguration.Enrich.WithSensitiveDataMasking(options =>
                {
                    options.MaskProperties = ["Password", "Email", "PhoneNumber"];
                });

                // Adiciona labels do Loki via código
                var serilogUsing = configuration.GetSection("Serilog:Using").Get<string[]>() ?? [];
                var useLoki = Array.Exists(serilogUsing, s => s == "Serilog.Sinks.Grafana.Loki");
                if (useLoki)
                {
                    var grafanaLokiUrl = configuration["Serilog:WriteTo:1:Args:uri"];
                    if (string.IsNullOrEmpty(grafanaLokiUrl))
                        throw new ArgumentNullException(nameof(grafanaLokiUrl), "GrafanaLokiUrl configuration is required.");

                    Log.Information("grafanaLokiUrl:", grafanaLokiUrl);
                    Log.Information("username:", configuration["Serilog:WriteTo:1:Args:credentials:username"]);
                    Log.Information("env:", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));
                    Log.Information("GRAFANA_API_TOKEN:", Environment.GetEnvironmentVariable("GRAFANA_API_TOKEN"));

                    loggerConfiguration.WriteTo.GrafanaLoki(
                        uri: grafanaLokiUrl,
                        credentials: new LokiCredentials
                        {
                            Login = configuration["Serilog:WriteTo:1:Args:credentials:username"],
                            Password = Environment.GetEnvironmentVariable("GRAFANA_API_TOKEN")
                        },
                        labels: new[]
                        {
                            new LokiLabel { Key = "app", Value = "tccloudgames-app" },
                            new LokiLabel { Key = "env", Value = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development" }
                        }
                    );
                }
            });
        }
    }
}