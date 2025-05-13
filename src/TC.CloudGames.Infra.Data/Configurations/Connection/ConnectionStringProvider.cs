using Microsoft.Extensions.Options;

namespace TC.CloudGames.Infra.Data.Configurations.Connection
{
    public sealed class ConnectionStringProvider : IConnectionStringProvider
    {
        private readonly DatabaseSettings _dbSettings;

        public ConnectionStringProvider(IOptions<DatabaseSettings> dbSettings)
        {
            _dbSettings = dbSettings.Value;
        }

        public string ConnectionString
        {
            get
            {
                var host = Environment.GetEnvironmentVariable("DB_HOST") ?? _dbSettings.Host;
                var port = Environment.GetEnvironmentVariable("DB_PORT") ?? _dbSettings.Port;
                var database = Environment.GetEnvironmentVariable("DB_NAME") ?? _dbSettings.Name;
                var username = Environment.GetEnvironmentVariable("DB_USER") ?? _dbSettings.User;
                var password = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? _dbSettings.Password;

                return $"Host={host};Port={port};Database={database};Username={username};Password={password}";
            }
        }
    }
}
