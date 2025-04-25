using TC.CloudGames.Domain.Abstractions;

namespace TC.CloudGames.Domain.Platform
{
    /// <summary>
    /// This class will be used in a future version and have its own repository and domain tables
    /// </summary>
    public sealed class Platform : Entity
    {
        public string Name { get; private set; }

        private Platform()
        {
            //EF Core
        }

        private Platform(Guid id, string name)
            : base(id)
        {
            Id = id;
            Name = name;
        }

        public static Platform Create(string name)
        {
            return new Platform(Guid.NewGuid(), name);
        }
    }
}