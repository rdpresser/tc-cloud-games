using ZiggyCreatures.Caching.Fusion;

namespace TC.CloudGames.CrossCutting.Commons.Caching;

public static class CacheOptions
{
    public static FusionCacheEntryOptions DefaultExpiration
    {
        get
        {
            return new()
            {
                Duration = TimeSpan.FromSeconds(20),
                DistributedCacheDuration = TimeSpan.FromSeconds(30)
            };
        }
    }
}