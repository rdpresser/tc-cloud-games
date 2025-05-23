using Shouldly;
using TC.CloudGames.Application.Users.Login;

namespace TC.CloudGames.Application.Tests.Users.Login;

public class LoginUserCommandValidatorTests
{
    private readonly LoginUserCommandValidator _validator = new();

    [Fact]
    public void Should_Return_Error_When_Email_Is_Empty()
    {
        // Arrange
        const string email = "";
        const string password = "senha123";
        var command = new LoginUserCommand(email, password);
        
        // Act
        var result = _validator.Validate(command);
        
        // Assert
        result.Errors.ShouldContain(e => e.PropertyName == nameof(LoginUserCommand.Email));
    }
    
    [Fact]
    public void Should_Return_Error_When_Password_Is_Invalid()
    {
        // Arrange
        const string email = "test@email.com";
        const string password = "abc";
        var command = new LoginUserCommand(email, password);
        
        // Act
        var result = _validator.Validate(command);
        
        result.Errors.ShouldContain(e => e.PropertyName == nameof(LoginUserCommand.Password));
    }
    
    [Fact]
    public void Should_Pass_When_Data_Is_Valid()
    {
        // Arrange
        const string email = "test@email.com";
        const string password = "J35!8G0+eP8z";
        var command = new LoginUserCommand(email, password);
        
        // Act
        var result = _validator.Validate(command);
        
        // Assert
        result.Errors.Count.ShouldBe(0);
    }

}