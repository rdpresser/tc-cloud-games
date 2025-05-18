using System.Diagnostics.CodeAnalysis;

namespace TC.CloudGames.Domain.Abstractions;

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

    public IReadOnlyList<IDomainEvent> GetDomainEvents()
    {
        return _domainEvents.ToList();
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    // Helper method to process value object creation results and collect errors
    protected static List<ValidationError> CollectValidationErrors(IEnumerable<IResult> results)
    {
        var errors = new List<ValidationError>();
        foreach (var result in results)
        {
            if (result.IsOk() || !result.ValidationErrors.Any())
            {
                continue;
            }
            errors.AddRange(result.ValidationErrors);
        }
        return errors;
    }
}