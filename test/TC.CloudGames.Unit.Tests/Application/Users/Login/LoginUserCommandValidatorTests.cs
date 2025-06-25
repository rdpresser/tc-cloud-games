using TC.CloudGames.Application.Users.Login;
using TC.CloudGames.Unit.Tests.Shared;

namespace TC.CloudGames.Unit.Tests.Application.Users.Login;

public class LoginUserCommandValidatorTests : BaseTest
{
    [Fact]
    public void Should_Return_Error_When_Email_Is_Empty()
    {
        LogTestStart(nameof(Should_Return_Error_When_Email_Is_Empty));

        // Arrange
        var validator = new LoginUserCommandValidator();
        var command = new LoginUserCommand(Email: "", Password: "ValidPass123!");

        // Act
        var result = validator.Validate(command);
        PrintValidationErrors(result.Errors);

        // Assert
        result.Errors.ShouldContain(e => e.PropertyName == nameof(LoginUserCommand.Email));
    }

    [Fact]
    public void Should_Return_Error_When_Password_Is_Invalid()
    {
        LogTestStart(nameof(Should_Return_Error_When_Password_Is_Invalid));

        // Arrange
        var email = CreateValidEmail;
        const string password = "abc";
        var command = new LoginUserCommand(email, password);
        var validator = new LoginUserCommandValidator();

        // Act
        var result = validator.Validate(command);
        PrintValidationErrors(result.Errors);

        // Assert
        result.Errors.ShouldContain(e => e.PropertyName == nameof(LoginUserCommand.Password));
    }

    [Fact]
    public void Should_Pass_When_Data_Is_Valid()
    {
        LogTestStart(nameof(Should_Pass_When_Data_Is_Valid));

        // Arrange
        string email = CreateValidEmail;
        string password = CreateValidPassword;
        var command = new LoginUserCommand(email, password);
        var validator = new LoginUserCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        result.Errors.Count.ShouldBe(0);
    }

}