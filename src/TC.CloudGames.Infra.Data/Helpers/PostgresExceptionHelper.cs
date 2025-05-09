using Npgsql;
using TC.CloudGames.Infra.Data.Exceptions;

namespace TC.CloudGames.Infra.Data.Helpers
{
    internal static class PostgresExceptionHelper
    {
        public static Exception ConvertException(PostgresException ex)
        {
            if (PostgresDuplicateKeyException.IsDuplicateKeyError(ex))
            {
                return new PostgresDuplicateKeyException(
                    message: $"Violação de chave única: {ex.MessageText}",
                    constraintName: ex.ConstraintName ?? "Desconhecido",
                    tableName: ex.TableName ?? "Desconhecida",
                    sqlState: ex.SqlState ?? "0",
                    columnName: ex.ColumnName ?? "Desconhecido",
                    innerException: ex);
            }

            return ex;
        }
    }
}