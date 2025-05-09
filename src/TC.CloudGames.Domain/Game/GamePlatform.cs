namespace TC.CloudGames.Domain.Game
{
    /// <summary>
    /// This class will be used in a future version and have its own repository and domain tables
    /// This class represents a many-to-many relationship between Game and Platform with composite key.
    /// https://medium.com/radzenhq/how-to-entity-framework-core-relationships-composite-keys-foreign-keys-data-annotations-code-f4b238086463
    /// </summary>
    public sealed class GamePlatform
    {
        public Guid GameId { get; private set; }
        //public virtual Game Game { get; set; }

        public Guid PlatformId { get; private set; }
        //public virtual Platform.Platform Platform { get; set; }

        private GamePlatform()
        {
            //EF Core
        }

        private GamePlatform(Guid gameId, Guid platformId)
        {
            GameId = gameId;
            PlatformId = platformId;
        }

        public static GamePlatform Create(Guid gameId, Guid platformId)
        {
            return new GamePlatform(gameId, platformId);
        }
    }
}
