namespace TC.CloudGames.Domain.Abstractions
{
    public abstract class Entity(Guid id)
    {
        public Guid Id { get; init; } = id;
    }
}
