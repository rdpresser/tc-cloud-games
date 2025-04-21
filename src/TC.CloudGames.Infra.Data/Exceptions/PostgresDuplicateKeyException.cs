using Npgsql;
using TC.CloudGames.Domain.Exceptions;

namespace TC.CloudGames.Infra.Data.Exceptions
{
    public class PostgresDuplicateKeyException : NpgsqlException, IDuplicateKeyException
    {
        public string ConstraintName { get; }
        public string TableName { get; }
        public string? ColumnName { get; }
        public override string? SqlState { get; }

        public PostgresDuplicateKeyException(
            string message = "Chave duplicada encontrada no PostgreSQL",
            string constraintName = "Desconhecido",
            string tableName = "Desconhecida",
            string sqlState = "0",
            string columnName = "Desconhecido",
            Exception? innerException = null)
            : base(message, innerException)
        {
            ConstraintName = constraintName;
            TableName = tableName;
            SqlState = sqlState;
            ColumnName = columnName;
        }

        public static bool IsDuplicateKeyError(PostgresException ex)
        {
            return ex.SqlState == "23505"; // Código específico para violação de unique constraint
        }
    }
}