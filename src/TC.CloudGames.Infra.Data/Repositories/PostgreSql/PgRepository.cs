using TC.CloudGames.Infra.Data.Configurations.Connection;

namespace TC.CloudGames.Infra.Data.Repositories.PostgreSql;

public abstract class PgRepository
{
    protected readonly IPgConnectionProvider ConnectionProvider;

    public PgRepository(IPgConnectionProvider connectionProvider)
    {
        ConnectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
    }
}