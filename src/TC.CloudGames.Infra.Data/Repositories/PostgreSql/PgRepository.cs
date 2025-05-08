using TC.CloudGames.Infra.Data.Configurations.Connection;

namespace TC.CloudGames.Infra.Data.Repositories.PostgreSql;

public abstract class PgRepository
{
    protected readonly IPgDbConnectionProvider ConnectionProvider;

    protected PgRepository(IPgDbConnectionProvider connectionProvider)
    {
        ConnectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
    }
}