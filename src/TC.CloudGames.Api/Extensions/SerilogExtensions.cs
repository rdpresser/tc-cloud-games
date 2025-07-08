using Serilog.Enrichers.Sensitive;
using Serilog.Events;
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

                var timeZoneId = configuration["TimeZone"] ?? "UTC";
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

                // Verifica se o Loki está configurado no appsettings
                var serilogUsing = configuration.GetSection("Serilog:Using").Get<string[]>() ?? [];
                var useLoki = Array.Exists(serilogUsing, s => s == "Serilog.Sinks.Grafana.Loki");

                loggerConfiguration
                    .WriteTo.Logger(lc =>
                    {
                        lc.MinimumLevel.Debug()
                          .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                          .Enrich.FromLogContext()
                          .Enrich.WithProperty("Application", "TC.CloudGames.Api")
                          .Enrich.WithProperty("Environment", hostContext.HostingEnvironment.EnvironmentName)
                          .Enrich.With(new UtcToLocalTimeEnricher(timeZone))
                          .WriteTo.Console(outputTemplate: "[{LocalTimestamp}] {Level:u3} {Message:lj}{NewLine}{Exception}");

                        if (useLoki)
                        {
                            var grafanaLokiUrl = configuration["GrafanaLokiUrl"];
                            if (string.IsNullOrEmpty(grafanaLokiUrl))
                                throw new ArgumentNullException(nameof(grafanaLokiUrl), "GrafanaLokiUrl configuration is required.");

                            lc.WriteTo.GrafanaLoki(
                                uri: grafanaLokiUrl,
                                credentials: new LokiCredentials
                                {
                                    Login = "1267846",
                                    Password = Environment.GetEnvironmentVariable("GRAFANA_API_TOKEN")
                                },
                                labels: [
                                    new LokiLabel { Key = "app", Value = "tccloudgames-app" },
                                    new LokiLabel { Key = "env", Value = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development" }
                                ]
                            );
                        }
                    })
                    .Enrich.WithSensitiveDataMasking(options =>
                    {
                        options.MaskProperties = ["Password", "Email", "PhoneNumber"];
                    });
            });
        }
    }
}