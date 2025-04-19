using Ardalis.Result;

namespace TC.CloudGames.Application.Users.CreateUser;

internal sealed class CreateUserCommandHandler : Abstractions.Messaging.ICommandHandler<CreateUserCommand, CreateUserResponse>
{
    //private readonly IUserRepository _userRepository;
    private readonly CreateUserMapper _mapper;

    public CreateUserCommandHandler(CreateUserMapper mapper)
    {
        //_userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<Result<CreateUserResponse>> ExecuteAsync(CreateUserCommand command, CancellationToken ct)
    {
        await Task.CompletedTask; // Simulate async work

        var entity = _mapper.ToEntity(command);

        return Result<CreateUserResponse>.Error("An error occured during the execution-2");

        //var response = _mapper.FromEntity(entity);
        //return Result<CreateUserResponse>.Success(response);
    }
}