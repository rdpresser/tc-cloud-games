using Npgsql;

namespace TC.CloudGames.Infra.Data.Configurations.Connection;

public sealed class PgDbConnectionProvider : IPgDbConnectionProvider, IDisposable, IAsyncDisposable
{
    private readonly IConnectionStringProvider _connectionStringProvider;
    private NpgsqlConnection? _connection;
    private bool _disposed;

    public PgDbConnectionProvider(IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider ??
                                    throw new ArgumentNullException(nameof(connectionStringProvider));
    }

    public NpgsqlConnection CreateConnection()
    {
        ObjectDisposedException.ThrowIf(_disposed, nameof(PgDbConnectionProvider));

        _connection = new NpgsqlConnection(_connectionStringProvider.ConnectionString);
        _connection.Open();

        return _connection;
    }

    public async Task<NpgsqlConnection> CreateConnectionAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, nameof(PgDbConnectionProvider));

        _connection = new NpgsqlConnection(_connectionStringProvider.ConnectionString);
        await _connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        return _connection;
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
    }

    private async ValueTask DisposeAsyncCore()
    {
        if (!_disposed)
        {
            if (_connection != null)
            {
                await _connection.DisposeAsync().ConfigureAwait(false);
                _connection = null;
            }

            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Dispose managed resources
                _connection?.Dispose();
            }

            _connection = null;
            _disposed = true;
        }
    }
}