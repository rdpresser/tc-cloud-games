﻿using TC.CloudGames.Application.Users.Login;

namespace TC.CloudGames.Unit.Tests.Application.Users.Login;

public class LoginUserCommandValidatorTests
{
    private readonly Faker _faker;
    private readonly LoginUserCommandValidator _validator = new();

    public LoginUserCommandValidatorTests()
    {
        _faker = new Faker();
    }

    [Fact]
    public void Should_Return_Error_When_Email_Is_Empty()
    {
        // Arrange
        const string email = "";
        var password = _faker.Internet.Password() + "!";
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
        var email = _faker.Internet.Email();
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
        const string email = "test@email.com";
        const string password = "J35!8G0+eP8z";
        var command = new LoginUserCommand(email, password);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.Errors.Count.ShouldBe(0);
    }

}