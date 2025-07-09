using System.Diagnostics.CodeAnalysis;
using Serilog.Enrichers.Sensitive;
using Serilog.Events;

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
                    })
                    .Enrich.WithSensitiveDataMasking(options =>
                    {
                        options.MaskProperties = ["Password", "Email", "PhoneNumber"];
                    });
            });
        }
    }
}