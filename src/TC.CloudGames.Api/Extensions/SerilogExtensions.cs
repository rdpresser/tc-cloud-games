using System.Diagnostics.CodeAnalysis;
using Serilog.Enrichers.Sensitive;

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

                // Se quiser manter o timezone customizado via código, pode adicionar só isso:
                var timeZoneId = configuration["TimeZone"] ?? "UTC";
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                loggerConfiguration.Enrich.With(new UtcToLocalTimeEnricher(timeZone));

                // Se quiser manter o masking via código, pode deixar aqui:
                loggerConfiguration.Enrich.WithSensitiveDataMasking(options =>
                {
                    options.MaskProperties = ["Password", "Email", "PhoneNumber"];
                });
            });
        }
    }
}