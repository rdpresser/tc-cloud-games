using Microsoft.Extensions.Options;

namespace TC.CloudGames.Infra.CrossCutting.Commons.Caching
{
    public sealed class CacheProvider : ICacheProvider
    {
        private readonly CacheSettings _cacheSettings;

        public CacheProvider(IOptions<CacheSettings> cacheSettings)
        {
            _cacheSettings = cacheSettings.Value;
        }

        public string InstanceName => _cacheSettings.InstanceName;

        public string ConnectionString
        {
            get
            {
                var host = Environment.GetEnvironmentVariable("CACHE_HOST") ?? _cacheSettings.Host;
                var port = Environment.GetEnvironmentVariable("CACHE_PORT") ?? _cacheSettings.Port;
                var password = Environment.GetEnvironmentVariable("CACHE_PASSWORD") ?? _cacheSettings.Password;

                if (string.IsNullOrWhiteSpace(password))
                {
                    // For localhost or unsecured Redis
                    return $"{host}:{port}";
                }

                // For secured Redis with password
                return $"{host}:{port},password={password},ssl=True,abortConnect=False";
            }
        }
    }
}
