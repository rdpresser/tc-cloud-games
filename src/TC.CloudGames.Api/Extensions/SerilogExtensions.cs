using Serilog;
using Serilog.Events;

namespace TC.CloudGames.Api.Extensions
{
    namespace TC.CloudGames.Api.Extensions
    {
        public static class SerilogExtensions
        {
            public static IHostBuilder UseCustomSerilog(this IHostBuilder hostBuilder, IConfiguration configuration)
            {
                return hostBuilder.UseSerilog((hostContext, services, loggerConfiguration) =>
                {
                    loggerConfiguration.ReadFrom.Configuration(hostContext.Configuration);

                    var timeZoneId = configuration["TimeZone"] ?? "UTC"; // Default to UTC if not specified
                    var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

                    loggerConfiguration
                        .WriteTo.Logger(lc => lc
                            .MinimumLevel.Debug()
                            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                            .Enrich.FromLogContext()
                            .Enrich.With(new UtcToLocalTimeEnricher(timeZone)) // Pass the timezone to the enricher
                            .WriteTo.Console(outputTemplate: "[{LocalTimestamp}] {Level:u3} {Message:lj}{NewLine}{Exception}"));
                });
            }
        }
    }

}
