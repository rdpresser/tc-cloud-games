using TC.CloudGames.Application.Users.Login;
using TC.CloudGames.Unit.Tests.Fakes;

namespace TC.CloudGames.Unit.Tests.Application.Users.Login;

public class LoginUserCommandValidatorTests
{
    private readonly LoginUserCommandValidator _validator = new();

    [Fact]
    public void Should_Return_Error_When_Email_Is_Empty()
    {
        // Arrange
        var userValid = FakeUserData.UserValid();
        
        const string email = "";
        var password = userValid.Password;
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
        var userValid = FakeUserData.UserValid();
        
        var email = userValid.Email;
        const string password = "abc";
        var command = new LoginUserCommand(email, password);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.Errors.ShouldContain(e => e.PropertyName == nameof(LoginUserCommand.Password));
    }

    [Fact]
    public void Should_Pass_When_Data_Is_Valid()
    {
        // Arrange
        var userValid = FakeUserData.UserValid();

        var command = new LoginUserCommand(userValid.Email, userValid.Password);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.Errors.Count.ShouldBe(0);
    }

}