using Npgsql;

namespace TC.CloudGames.Infra.Data.Configurations.Connection;

public interface IPgConnectionProvider
{
    NpgsqlConnection CreateConnection();
    Task<NpgsqlConnection> CreateConnectionAsync(CancellationToken cancellationToken = default);
}