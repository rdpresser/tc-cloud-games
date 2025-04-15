using Ardalis.Result;
using Microsoft.Extensions.DependencyInjection;

namespace TC.CloudGames.Application.Users.CreateUser;

internal sealed class CreateUserCommandHandler : Abstractions.Messaging.ICommandHandler<CreateUserCommand, CreateUserResponse>
{
    private readonly IServiceScopeFactory _scopeFactory;

    public CreateUserCommandHandler(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<Result<CreateUserResponse>> ExecuteAsync(CreateUserCommand command, CancellationToken ct)
    {
        await Task.CompletedTask; // Simulate async work

        var mapper = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<CreateUserMapper>();
        var entity = mapper.ToEntity(command);


        var response = mapper.FromEntity(entity);
        return Result<CreateUserResponse>.Success(response);
    }
}