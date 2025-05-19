using Serilog.Enrichers.Sensitive;
using Serilog.Events;
using System.Diagnostics.CodeAnalysis;

namespace TC.CloudGames.Api.Extensions
{
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

                    var timeZoneId = configuration["TimeZone"] ?? "UTC"; // Default to UTC if not specified
                    var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

                    loggerConfiguration
                        .WriteTo.Logger(lc => lc
                            .MinimumLevel.Debug()
                            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                            .Enrich.FromLogContext()
                            .Enrich.WithProperty("Application", "TC.CloudGames.Api")
                            .Enrich.WithProperty("Environment", hostContext.HostingEnvironment.EnvironmentName)
                            .Enrich.With(new UtcToLocalTimeEnricher(timeZone)) // Pass the timezone to the enricher
                            .WriteTo.Console(outputTemplate: "[{LocalTimestamp}] {Level:u3} {Message:lj}{NewLine}{Exception}"))
                            .Enrich.WithSensitiveDataMasking(options =>
                            {
                                options.MaskProperties = ["Password", "Email", "PhoneNumber"];
                            });
                });
            }
        }
    }

}
