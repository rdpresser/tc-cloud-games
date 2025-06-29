﻿namespace TC.CloudGames.Domain.Exceptions;

public interface IDuplicateKeyViolation
{
    string Message { get; }
    string ConstraintName { get; }
    string TableName { get; }
    string? ColumnName { get; }
    string? SqlState { get; }
    Exception? InnerException { get; }
}