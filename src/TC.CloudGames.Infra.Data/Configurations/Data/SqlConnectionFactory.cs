using Npgsql;
using System.Data;
using TC.CloudGames.Application.Abstractions.Data;

namespace TC.CloudGames.Infra.Data.Configurations.Data
{
    public sealed class SqlConnectionFactory : ISqlConnectionFactory
    {
        private readonly IDatabaseConnectionProvider _connectionProvider;

        public SqlConnectionFactory(IDatabaseConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
        }

        public IDbConnection CreateConnection()
        {
            var connection = new NpgsqlConnection(_connectionProvider.ConnectionString);
            connection.Open();

            return connection;
        }

        public async Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default)
        {
            var connection = new NpgsqlConnection(_connectionProvider.ConnectionString);
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

            return connection;
        }
    }
}
