using Ardalis.Result;
using FakeItEasy;
using FastEndpoints;
using System.Reflection.Metadata;
using TC.CloudGames.Application.Abstractions.Data;
using TC.CloudGames.Application.Abstractions.Messaging;
using TC.CloudGames.Application.Users.Login;
using TC.CloudGames.Domain.User;
using TC.CloudGames.Domain.User.Abstractions;
using TC.CloudGames.Infra.CrossCutting.Commons.Authentication;

namespace TC.CloudGames.Application.Tests.Users.Login;

public class LoginUserCommandHandlerTests
{
    [Fact]
    public async Task LoginUserCommandHandler_LoginUser_Ok()
    {
        // Arrange
        Factory.RegisterTestServices(_ => {});
        
        var loginReq = new LoginUserCommand(Email: "teste@exemplo.com", Password: "senha123");
        var loginRes = new LoginUserResponse("<jwt-token>", "teste@exemplo.com");
        
        var fakeHandler = A.Fake<CommandHandler<LoginUserCommand, LoginUserResponse, Domain.User.User, IUserEfRepository>>();
        A.CallTo(() => fakeHandler.ExecuteAsync(A<LoginUserCommand>.Ignored, A<CancellationToken>.Ignored))
            .Returns(Task.FromResult(Result<LoginUserResponse>.Success(loginRes)));
        
        // Act
        var result = await fakeHandler.ExecuteAsync(loginReq, CancellationToken.None);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("<jwt-token>", result.Value.JwtToken);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnValidationError_WhenUserNotFound()
    {
        // Arrange
        Factory.RegisterTestServices(_ => { });

        var unitOfWork = A.Fake<IUnitOfWork>();
        var userRepository = A.Fake<IUserEfRepository>();
        var tokenProvider = A.Fake<ITokenProvider>();
        var handler = new LoginUserCommandHandler(unitOfWork, userRepository, tokenProvider);

        var command = new LoginUserCommand("test@email.com", "password");
        A.CallTo(() => userRepository.GetByEmailWithPasswordAsync(command.Email, command.Password, A<CancellationToken>._))
            .Returns(Task.FromResult<User?>(null));

        // Act
        var result = await handler.ExecuteAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Errors);
    }
}