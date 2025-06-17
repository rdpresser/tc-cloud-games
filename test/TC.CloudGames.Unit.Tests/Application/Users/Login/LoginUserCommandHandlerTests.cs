using TC.CloudGames.Application.Users.Login;
using TC.CloudGames.Domain.Aggregates.User;
using TC.CloudGames.Domain.Aggregates.User.Abstractions;
using TC.CloudGames.Infra.CrossCutting.Commons.Authentication;
using TC.CloudGames.Unit.Tests.Fakes;

namespace TC.CloudGames.Unit.Tests.Application.Users.Login;

public class LoginUserCommandHandlerTests
{

    [Fact]
    public async Task LoginUserCommandHandler_LoginUser_Ok()
    {
        // Arrange
        Factory.RegisterTestServices(_ => { });

        var userValid = FakeUserData.UserValid();

        var email = userValid.Email;
        var password = userValid.Password;

        var loginReq = new LoginUserCommand(Email: email, Password: password);
        var loginRes = new LoginUserResponse("<jwt-token>", email);

        var fakeHandler = A.Fake<CommandHandler<LoginUserCommand, LoginUserResponse, User, IUserEfRepository>>();
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
        
        var userValid = FakeUserData.UserValid();

        var unitOfWork = A.Fake<IUnitOfWork>();
        var userRepository = A.Fake<IUserEfRepository>();
        var tokenProvider = A.Fake<ITokenProvider>();
        var handler = new LoginUserCommandHandler(unitOfWork, userRepository, tokenProvider);

        var email = userValid.Email;
        var password = "password";

        var command = new LoginUserCommand(email, password);
        A.CallTo(() => userRepository.GetByEmailWithPasswordAsync(command.Email, command.Password, A<CancellationToken>._))
            .Returns(Task.FromResult<User?>(null));

        // Act
        var result = await handler.ExecuteAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Errors);
    }
}