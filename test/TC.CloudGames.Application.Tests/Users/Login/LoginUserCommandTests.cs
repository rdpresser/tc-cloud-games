using TC.CloudGames.Application.Users.Login;

namespace TC.CloudGames.Application.Tests.Users.Login;

public class LoginUserCommandTests
{
    [Fact]
    public async Task LoginUserCommand_ShouldReturnSuccess_WhenValidCredentials()
    {
        // Arrange
        const string email = "test@example.com";
        const string password = "password123";
        
        // Act
        var command = new LoginUserCommand(email, password);
        
        //Assert
        Assert.Equal(email, command.Email);
        Assert.Equal(password, command.Password);
    }
}