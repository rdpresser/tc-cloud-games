using Npgsql;

namespace TC.CloudGames.Infra.Data.Configurations.Connection;

public interface IPgDbConnectionProvider
{
    NpgsqlConnection CreateConnection();
    Task<NpgsqlConnection> CreateConnectionAsync(CancellationToken cancellationToken = default);
    void Dispose();
    ValueTask DisposeAsync();
}