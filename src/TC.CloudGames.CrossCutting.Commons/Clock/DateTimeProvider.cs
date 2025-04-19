namespace TC.CloudGames.CrossCutting.Commons.Clock
{
    public sealed class DateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
