using TC.CloudGames.Domain.Exceptions;

namespace TC.CloudGames.Application.Tests.Users.CreateUser
{
    public class DuplicateKeyViolationException : Exception, IDuplicateKeyViolation
    {
        public string ConstraintName { get; }
        public string TableName { get; }
        public string? ColumnName { get; }
        public string? SqlState { get; }
        public Exception? InnerException { get; }

        public DuplicateKeyViolationException(string message, string tableName, string? columnName = null)
            : base(message)
        {
            TableName = tableName;
            ColumnName = columnName;
            ConstraintName = "UNIQUE";
        }
    }
}
