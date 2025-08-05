using TC.CloudGames.User.API.Extensions;
using TC.CloudGames.User.Application.Ports;
using TC.CloudGames.User.Domain.Aggregates;

var builder = WebApplication.CreateBuilder(args);

// Adicionar serviços do container
builder.Services.AddUserServices(builder.Configuration);

// Adicionar serviços para API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configurar o pipeline de requisições HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Endpoints da API de usuários usando APIs mínimas
app.MapPost("/api/users", async (CreateUserRequest request, IUserRepository userRepository) =>
{
    var userId = Guid.NewGuid();
    var user = UserAggregate.Create(userId, request.Email, request.Username);

    await userRepository.SaveAsync(user);

    return Results.Created($"/api/users/{userId}", new { Id = userId });
})
.WithName("CreateUser")
.WithOpenApi();

app.MapGet("/api/users/{id:guid}", async (Guid id, IUserRepository userRepository) =>
{
    var user = await userRepository.GetByIdAsync(id);

    if (user == null)
        return Results.NotFound();

    var response = new UserResponse
    {
        Id = user.Id,
        Email = user.Email,
        Username = user.Username,
        CreatedAt = user.CreatedAt,
        UpdatedAt = user.UpdatedAt,
        IsActive = user.IsActive
    };

    return Results.Ok(response);
})
.WithName("GetUser")
.WithOpenApi();

app.MapGet("/api/users", async (IUserRepository userRepository) =>
{
    var users = await userRepository.GetAllAsync();

    var response = users.Select(user => new UserResponse
    {
        Id = user.Id,
        Email = user.Email,
        Username = user.Username,
        CreatedAt = user.CreatedAt,
        UpdatedAt = user.UpdatedAt,
        IsActive = user.IsActive
    });

    return Results.Ok(response);
})
.WithName("GetAllUsers")
.WithOpenApi();

app.MapPut("/api/users/{id:guid}", async (Guid id, UpdateUserRequest request, IUserRepository userRepository) =>
{
    var user = await userRepository.GetByIdAsync(id);

    if (user == null)
        return Results.NotFound();

    user.UpdateInfo(request.Email, request.Username);

    await userRepository.SaveAsync(user);

    return Results.NoContent();
})
.WithName("UpdateUser")
.WithOpenApi();

app.MapDelete("/api/users/{id:guid}", async (Guid id, IUserRepository userRepository) =>
{
    var user = await userRepository.GetByIdAsync(id);

    if (user == null)
        return Results.NotFound();

    user.Deactivate();

    await userRepository.SaveAsync(user);

    return Results.NoContent();
})
.WithName("DeactivateUser")
.WithOpenApi();

// Endpoint de health check
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }))
.WithName("HealthCheck")
.WithOpenApi();

await app.RunAsync();

// DTOs
public class CreateUserRequest
{
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
}

public class UpdateUserRequest
{
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
}

public class UserResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; }
}
