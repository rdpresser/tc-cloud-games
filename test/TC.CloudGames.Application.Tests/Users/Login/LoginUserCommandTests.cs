using TC.CloudGames.Application.Users.Login;

namespace TC.CloudGames.Application.Tests.Users.Login;

public class LoginUserCommandTests
{
    [Fact]
    public async Task LoginUserCommand_ShouldReturnSuccess_WhenValidCredentials()
    {
        // Arrange
        var email = "teste@exemplo.com";
        var senha = "senha123";
        
        // Act
        var command = new LoginUserCommand(email, senha);
        
        //Assert
        Assert.Equal(email, command.Email);
        Assert.Equal(senha, command.Password);
    }
}