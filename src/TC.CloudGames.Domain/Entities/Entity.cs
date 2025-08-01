﻿using System.Diagnostics.CodeAnalysis;

namespace TC.CloudGames.Domain.Entities;

[ExcludeFromCodeCoverage]
public abstract class Entity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    protected Entity()
    {
        //EF Core
    }

    protected Entity(Guid id)
    {
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
    }

    public Guid Id { get; protected init; }
    public DateTime CreatedOnUtc { get; private set; }

    public void SetCreatedOnUtc(DateTime createdOnUtc)
    {
        CreatedOnUtc = createdOnUtc;
    }

    public IReadOnlyList<IDomainEvent> DomainEvents => [.. _domainEvents];

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    // Helper to ensure Result<T> is always non-null, even if the object is null
    protected static Result<T> EnsureResult<T>(T? result, string valueObjectName) where T : class
    {
        if (result is not null)
        {
            return Result<T>.Success(result);
        }

        return Result<T>.Invalid(new ValidationError(
                identifier: valueObjectName,
                errorMessage: $"Value object '{valueObjectName}' creation failed.",
                errorCode: "ValueObjectCreation.Error",
                severity: ValidationSeverity.Error));
    }

    // Helper to ensure Result<T> is always non-null, even if the object is null
    protected static Result<T> EnsureResult<T>(Result<T>? result, string valueObjectName) where T : class
    {
        if (result is not null)
        {
            return result;
        }

        return Result<T>.Invalid(new ValidationError(
                identifier: valueObjectName,
                errorMessage: $"Value object '{valueObjectName}' creation failed.",
                errorCode: "ValueObjectCreation.Error",
                severity: ValidationSeverity.Error));
    }

    // Helper method to process value object creation results and collect errors
    protected static IReadOnlyList<ValidationError> CollectValidationErrors(IEnumerable<IResult> results)
    {
        ArgumentNullException.ThrowIfNull(results);

        var errors = new List<ValidationError>();
        foreach (var result in results)
        {
            if (result == null)
            {
                errors.Add(new ValidationError(
                    identifier: "Unknown",
                    errorMessage: "Value object creation failed.",
                    errorCode: "ValueObjectCreation.Error",
                    severity: ValidationSeverity.Error));

                continue;
            }

            if (result.IsOk() || !result.ValidationErrors.Any())
            {
                continue;
            }
            errors.AddRange(result.ValidationErrors);
        }
        return errors;
    }
}