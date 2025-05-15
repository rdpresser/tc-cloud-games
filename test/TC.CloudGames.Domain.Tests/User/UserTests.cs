using Ardalis.Result;
using Bogus;
using NSubstitute;
using Shouldly;
using TC.CloudGames.Domain.User.Abstractions;
using TC.CloudGames.Domain.User;
using DomainUser = TC.CloudGames.Domain.User.User;

namespace TC.CloudGames.Domain.Tests.User;

public class UserTests
{
    private readonly Faker _faker;
    private readonly IUserEfRepository _userEfRepository;

    public UserTests()
    {
        _faker = new Faker();
        _userEfRepository = Substitute.For<IUserEfRepository>();
    }
    
    [Fact]
    public async Task Create_User_Should_Return_Invalid_When_Required_Value_Objects_Are_Empty()
    {
        // Arrange
        var userEfRepository = Substitute.For<IUserEfRepository>();
        userEfRepository.EmailExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(false)); // or true, depending on your test

        // Act
        var userResult = await DomainUser.CreateAsync(
            firstName: _faker.Name.FirstName(),
            lastName: _faker.Name.LastName(),
            email: string.Empty,
            password: string.Empty,
            role: string.Empty,
            userEfRepository
        );

        // Assert
        var errors = userResult.ValidationErrors;
        errors.ShouldNotBeNull()
            .ShouldNotBeEmpty();
        errors.ShouldBeOfType<List<ValidationError>>();
        errors.Count().ShouldBe(10);

        userResult.Status.ShouldBe(ResultStatus.Invalid);
        errors.Count(x => x.Identifier == nameof(DomainUser.FirstName)).ShouldBe(0);
        errors.Count(x => x.Identifier == nameof(DomainUser.LastName)).ShouldBe(0);
        errors.Count(x => x.Identifier == nameof(Email)).ShouldBe(2);
        errors.Count(x => x.Identifier == nameof(Password)).ShouldBe(6);
        errors.Count(x => x.Identifier == nameof(Role)).ShouldBe(2);
    }
    
    [Fact]
    public async Task Create_User_Should_Return_Success_When_All_Fields_Are_Valid()
    {
        // Arrange
        var firstName = _faker.Name.FirstName();
        var lastName = _faker.Name.LastName();
        var email = _faker.Internet.Email();
        var password = "Senha@123456";
        var role = "User";

        _userEfRepository.EmailExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(false));

        // Act
        var result = await DomainUser.CreateAsync(
            firstName: firstName,
            lastName: lastName,
            email: email,
            password: password,
            role: role,
            _userEfRepository
        );

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        result.Value.ShouldNotBeNull();
        result.Value.FirstName.ShouldBe(firstName);
        result.Value.LastName.ShouldBe(lastName);
        result.Value.Email.Value.ShouldBe(email);
    }

    [Fact]
    public async Task Create_User_Should_Return_Invalid_When_Email_Already_Exists()
    {
        // Arrange
        _userEfRepository.EmailExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(true));

        // Act
        var result = await DomainUser.CreateAsync(
            firstName: _faker.Name.FirstName(),
            lastName: _faker.Name.LastName(),
            email: _faker.Internet.Email(),
            password: "Senha@123456",
            role: "User",
            _userEfRepository
        );

        // Assert
        result.Status.ShouldBe(ResultStatus.Invalid);
        result.ValidationErrors.ShouldContain(x => x.Identifier == nameof(Email));
    }

    [Theory]
    [InlineData("senha123")] // Sem caractere especial
    [InlineData("Senha123")] // Sem caractere especial
    [InlineData("Senha@")] // Muito curta
    [InlineData("senha@123")] // Sem letra maiúscula
    public async Task Create_User_Should_Return_Invalid_When_Password_Format_Is_Invalid(string invalidPassword)
    {
        // Arrange
        _userEfRepository.EmailExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(false));

        // Act
        var result = await DomainUser.CreateAsync(
            firstName: _faker.Name.FirstName(),
            lastName: _faker.Name.LastName(),
            email: _faker.Internet.Email(),
            password: invalidPassword,
            role: "User",
            _userEfRepository
        );

        // Assert
        result.Status.ShouldBe(ResultStatus.Invalid);
        result.ValidationErrors.ShouldContain(x => x.Identifier == nameof(Password));
    }
}