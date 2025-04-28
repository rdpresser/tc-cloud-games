using Serilog.Core;
using Serilog.Events;

namespace TC.CloudGames.Api.Extensions
{
    public class UtcToLocalTimeEnricher : ILogEventEnricher
    {
        private readonly TimeZoneInfo _timeZone;

        public UtcToLocalTimeEnricher(TimeZoneInfo timeZone)
        {
            _timeZone = timeZone ?? throw new ArgumentNullException(nameof(timeZone));
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));

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