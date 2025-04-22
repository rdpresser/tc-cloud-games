using Npgsql;
using System.Data;
using TC.CloudGames.Application.Abstractions.Data;

namespace TC.CloudGames.Infra.Data.Configurations.Data
{
    public sealed class SqlConnectionFactory : ISqlConnectionFactory
    {
        private readonly string _connectionString;

        public SqlConnectionFactory(string connectionString)
        {
            var DB_HOST = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
            var DB_PORT = Environment.GetEnvironmentVariable("DB_PORT") ?? "54320";

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString), "Connection string cannot be null or empty.");
            }

            _connectionString = connectionString
                    .Replace("${DB_HOST}", DB_HOST)
                    .Replace("${DB_PORT}", DB_PORT);
        }

        public IDbConnection CreateConnection()
        {
            var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            return connection;
        }

        public async Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default)
        {
            var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            return connection;
        }
    }
}
