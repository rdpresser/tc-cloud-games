namespace TC.CloudGames.CrossCutting.Commons.Clock
{
    /// <summary>
    /// Inteface for providing the current date and time.
    /// Using interface is possible to mock and test the code more easily.
    /// </summary>
    public interface IDateTimeProvider
    {
        DateTime UtcNow { get; }
    }
}
