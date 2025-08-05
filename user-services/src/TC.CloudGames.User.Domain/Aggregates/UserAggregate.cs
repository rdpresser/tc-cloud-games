using System;
using System.Collections.Generic;
using System.Linq;
using Marten.Events;

namespace TC.CloudGames.User.Domain.Aggregates;

public class UserAggregate
{
    private readonly List<object> _uncommittedEvents = new();

    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string Username { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public bool IsActive { get; private set; }

    public IReadOnlyList<object> UncommittedEvents => _uncommittedEvents.AsReadOnly();

    // Construtor para reconstrução do agregado a partir de eventos
    public UserAggregate()
    {
    }

    // Factory method para criar um novo usuário
    public static UserAggregate Create(Guid id, string email, string username)
    {
        var aggregate = new UserAggregate();
        var @event = new UserCreatedEvent(id, email, username, DateTime.UtcNow);
        aggregate.ApplyEvent(@event);
        return aggregate;
    }

    // Factory method para recriar a partir de dados de projeção (para consultas eficientes)
    public static UserAggregate FromProjection(Guid id, string email, string username, DateTime createdAt, DateTime? updatedAt, bool isActive)
    {
        var aggregate = new UserAggregate
        {
            Id = id,
            Email = email,
            Username = username,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt,
            IsActive = isActive
        };
        return aggregate;
    }

    // Métodos de aplicação de eventos
    public void Apply(UserCreatedEvent @event)
    {
        Id = @event.Id;
        Email = @event.Email;
        Username = @event.Username;
        CreatedAt = @event.CreatedAt;
        IsActive = true;
    }

    public void Apply(UserUpdatedEvent @event)
    {
        Email = @event.Email;
        Username = @event.Username;
        UpdatedAt = @event.UpdatedAt;
    }

    public void Apply(UserDeactivatedEvent @event)
    {
        IsActive = false;
        UpdatedAt = @event.DeactivatedAt;
    }

    // Métodos de negócio
    public void UpdateInfo(string email, string username)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));
        
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty", nameof(username));

        var @event = new UserUpdatedEvent(Id, email, username, DateTime.UtcNow);
        ApplyEvent(@event);
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw new InvalidOperationException("User is already deactivated");

        var @event = new UserDeactivatedEvent(Id, DateTime.UtcNow);
        ApplyEvent(@event);
    }

    private void ApplyEvent(object @event)
    {
        _uncommittedEvents.Add(@event);
        
        // Apply the event to update the aggregate state
        switch (@event)
        {
            case UserCreatedEvent createdEvent:
                Apply(createdEvent);
                break;
            case UserUpdatedEvent updatedEvent:
                Apply(updatedEvent);
                break;
            case UserDeactivatedEvent deactivatedEvent:
                Apply(deactivatedEvent);
                break;
        }
    }

    public void MarkEventsAsCommitted()
    {
        _uncommittedEvents.Clear();
    }
}

// Eventos de domínio
public record UserCreatedEvent(Guid Id, string Email, string Username, DateTime CreatedAt);
public record UserUpdatedEvent(Guid Id, string Email, string Username, DateTime UpdatedAt);
public record UserDeactivatedEvent(Guid Id, DateTime DeactivatedAt);
