using Ardalis.Result;
using FakeItEasy;
using FastEndpoints;
using TC.CloudGames.Application.Abstractions.Messaging;
using TC.CloudGames.Application.Users.Login;
using TC.CloudGames.Domain.User.Abstractions;

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
}