using Npgsql;

namespace TC.CloudGames.Infra.Data.Configurations.Connection;

public sealed class PgConnectionProvider : IPgConnectionProvider, IAsyncDisposable, IDisposable
{
    private readonly IConnectionStringProvider _connectionStringProvider;
    private NpgsqlConnection _connection;
    private bool _disposed;

    public PgConnectionProvider(IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider ??
                                    throw new ArgumentNullException(nameof(connectionStringProvider));
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        if (_connection != null)
        {
            await _connection.DisposeAsync();
            _connection = null;
        }

        _disposed = true;
    }

    public void Dispose()
    {
        if (_disposed) return;
        if (_connection != null)
        {
            _connection.Dispose();
            _connection = null;
        }

        _disposed = true;
    }

    public NpgsqlConnection CreateConnection()
    {
        _connection = new NpgsqlConnection(_connectionStringProvider.ConnectionString);
        _connection.Open();

        return _connection;
    }

    public async Task<NpgsqlConnection> CreateConnectionAsync(CancellationToken cancellationToken = default)
    {
        _connection = new NpgsqlConnection(_connectionStringProvider.ConnectionString);
        await _connection!.OpenAsync(cancellationToken).ConfigureAwait(false);

        return _connection;
    }
}