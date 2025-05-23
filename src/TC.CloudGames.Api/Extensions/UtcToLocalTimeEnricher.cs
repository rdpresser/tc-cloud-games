using Serilog.Core;
using Serilog.Events;
using System.Diagnostics.CodeAnalysis;

namespace TC.CloudGames.Api.Extensions
{
    [ExcludeFromCodeCoverage]
    internal class UtcToLocalTimeEnricher : ILogEventEnricher
    {
        private readonly TimeZoneInfo _timeZone;

        public UtcToLocalTimeEnricher(TimeZoneInfo timeZone)
        {
            _timeZone = timeZone ?? throw new ArgumentNullException(nameof(timeZone));
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            ArgumentNullException.ThrowIfNull(logEvent);

            try
            {
                // Convert UTC to local time using the specified timezone
                var localTimestamp = TimeZoneInfo.ConvertTimeFromUtc(logEvent.Timestamp.UtcDateTime, _timeZone);

                // Add a custom property for the local timestamp
                var localTimestampProperty = propertyFactory.CreateProperty(
                    "LocalTimestamp",
                    localTimestamp.ToString("yyyy-MM-dd HH:mm:ss")
                );

                logEvent.AddOrUpdateProperty(localTimestampProperty);
            }
            catch (TimeZoneNotFoundException)
            {
                throw new InvalidOperationException($"The timezone '{_timeZone.Id}' is not recognized on this system.");
            }
            catch (InvalidTimeZoneException)
            {
                throw new InvalidOperationException($"The timezone '{_timeZone.Id}' is invalid or corrupted.");
            }
        }

    }
}