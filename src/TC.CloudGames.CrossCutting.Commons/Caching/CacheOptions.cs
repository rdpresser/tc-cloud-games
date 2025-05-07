using ZiggyCreatures.Caching.Fusion;

namespace TC.CloudGames.CrossCutting.Commons.Caching;

/// <summary>
///     Provides default cache options for FusionCache entries.
/// </summary>
public static class CacheOptions
{
    /// <summary>
    ///     Gets the default expiration settings for cache entries.
    /// </summary>
    /// <remarks>
    ///     - <see cref="FusionCacheEntryOptions.Duration" /> specifies the in-memory cache duration (20 seconds).
    ///     - <see cref="FusionCacheEntryOptions.DistributedCacheDuration" /> specifies the distributed cache duration (30
    ///     seconds).
    /// </remarks>
    public static FusionCacheEntryOptions DefaultExpiration =>
        new()
        {
            Duration = TimeSpan.FromSeconds(20),
            DistributedCacheDuration = TimeSpan.FromSeconds(30)
        };
}