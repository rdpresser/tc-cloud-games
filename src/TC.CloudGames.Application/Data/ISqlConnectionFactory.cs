using System.Data;

namespace TC.CloudGames.Application.Data
{
    public interface ISqlConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
