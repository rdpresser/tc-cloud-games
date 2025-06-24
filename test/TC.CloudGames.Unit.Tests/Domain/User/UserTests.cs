using TC.CloudGames.Domain.Aggregates.User.Abstractions;
using TC.CloudGames.Domain.Aggregates.User.ValueObjects;
using DomainUser = TC.CloudGames.Domain.Aggregates.User.User;

namespace TC.CloudGames.Unit.Tests.Domain.User;

public class UserTests
{
    private readonly Faker _faker;
    private readonly IUserEfRepository _userEfRepository;

    public UserTests()
    {
        _faker = new Faker();
        _userEfRepository = A.Fake<IUserEfRepository>();
    }

    [Fact]
    public async Task Create_User_Should_Return_Invalid_When_Required_Value_Objects_Are_Empty()
    {
        // Arrange
        A.CallTo(() => _userEfRepository.EmailExistsAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(Task.FromResult(false)); // or true, depending on your test

        // Act
        var userResult = await DomainUser.CreateAsync(builder =>
        {
            builder.FirstName = _faker.Name.FirstName();
            builder.LastName = _faker.Name.LastName();
            builder.Email = string.Empty;
            builder.Password = string.Empty;
            builder.Role = string.Empty;
        },
        _userEfRepository);

        // Assert
        var errors = userResult.ValidationErrors;
        errors.ShouldNotBeNull()
            .ShouldNotBeEmpty();
        errors.ShouldBeOfType<List<ValidationError>>();

        userResult.Status.ShouldBe(ResultStatus.Invalid);
        errors.Count(x => x.Identifier == nameof(DomainUser.FirstName)).ShouldBe(0);
        errors.Count(x => x.Identifier == nameof(DomainUser.LastName)).ShouldBe(0);

        errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Any(x => x.Identifier == nameof(Email) && x.ErrorCode == $"{nameof(Email)}.Required").ShouldBeTrue();
            errors.Any(x => x.Identifier == nameof(Email) && x.ErrorCode == $"{nameof(Email)}.InvalidFormat").ShouldBeTrue();
        });

        errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Any(x => x.Identifier == nameof(Password) && x.ErrorCode == $"{nameof(Password)}.Required").ShouldBeTrue();
            errors.Any(x => x.Identifier == nameof(Password) && x.ErrorCode == $"{nameof(Password)}.MinimumLength").ShouldBeTrue();
            errors.Any(x => x.Identifier == nameof(Password) && x.ErrorCode == $"{nameof(Password)}.Uppercase").ShouldBeTrue();
            errors.Any(x => x.Identifier == nameof(Password) && x.ErrorCode == $"{nameof(Password)}.Lowercase").ShouldBeTrue();
            errors.Any(x => x.Identifier == nameof(Password) && x.ErrorCode == $"{nameof(Password)}.Digit").ShouldBeTrue();
            errors.Any(x => x.Identifier == nameof(Password) && x.ErrorCode == $"{nameof(Password)}.SpecialCharacter").ShouldBeTrue();
        });
        
        errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Any(x => x.Identifier == nameof(Role) && x.ErrorCode == $"{nameof(Role)}.Required").ShouldBeTrue();
            errors.Any(x => x.Identifier == nameof(Role) && x.ErrorCode == $"{nameof(Role)}.InvalidRole").ShouldBeTrue();
        });
    }

    [Fact]
    public async Task Create_User_Using_Builder_Should_Return_Invalid_When_Required_Value_Objects_Are_Empty()
    {
        // Arrange
        A.CallTo(() => _userEfRepository.EmailExistsAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(Task.FromResult(false)); // or true, depending on your test

        var email = await Email.CreateAsync(builder => { }, _userEfRepository);
        var password = Password.Create(builder => { });
        var role = Role.Create(builder => { });

        // Act
        var userResult = DomainUser.CreateFromValueObjects(builder =>
        {
            builder.FirstName = _faker.Name.FirstName();
            builder.LastName = _faker.Name.LastName();
            builder.Email = email;
            builder.Password = password;
            builder.Role = role;
        });

        // Assert
        var errors = userResult.ValidationErrors;
        errors.ShouldNotBeNull()
            .ShouldNotBeEmpty();
        errors.ShouldBeOfType<List<ValidationError>>();
        
        userResult.Status.ShouldBe(ResultStatus.Invalid);
        errors.Count(x => x.Identifier == nameof(DomainUser.FirstName)).ShouldBe(0);
        errors.Count(x => x.Identifier == nameof(DomainUser.LastName)).ShouldBe(0);
        errors.Count(x => x.Identifier == nameof(Email) && x.ErrorCode == $"{nameof(Email)}.Required").ShouldBe(1);
        errors.Count(x => x.Identifier == nameof(Password)).ShouldBe(2);
        errors.Count(x => x.Identifier == nameof(Role)).ShouldBe(2);
    }

    [Fact]
    public async Task Create_User_Using_Builder_Result_Should_Return_Invalid_When_Required_Value_Objects_Are_Empty()
    {
        // Arrange
        A.CallTo(() => _userEfRepository.EmailExistsAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(Task.FromResult(false)); // or true, depending on your test

        var email = await Email.CreateAsync(builder => { }, _userEfRepository);
        var password = Password.Create(builder => { });
        var role = Role.Create(builder => { });

        // Act
        var userResult = DomainUser.CreateFromResult(builder =>
        {
            builder.FirstName = _faker.Name.FirstName();
            builder.LastName = _faker.Name.LastName();
            builder.Email = email;
            builder.Password = password;
            builder.Role = role;
        });

        // Assert
        var errors = userResult.ValidationErrors;
        errors.ShouldNotBeNull()
            .ShouldNotBeEmpty();
        errors.ShouldBeOfType<List<ValidationError>>();

        errors.Count(x => x.Identifier == nameof(DomainUser.FirstName)).ShouldBe(0);
        errors.Count(x => x.Identifier == nameof(DomainUser.LastName)).ShouldBe(0);

        errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Any(x => x.Identifier == nameof(Email) && x.ErrorCode == $"{nameof(Email)}.Required").ShouldBeTrue();
            errors.Any(x => x.Identifier == nameof(Email) && x.ErrorCode == $"{nameof(Email)}.InvalidFormat").ShouldBeTrue();
        });

        errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Any(x => x.Identifier == nameof(Password) && x.ErrorCode == $"{nameof(Password)}.Required").ShouldBeTrue();
            errors.Any(x => x.Identifier == nameof(Password) && x.ErrorCode == $"{nameof(Password)}.MinimumLength").ShouldBeTrue();
            errors.Any(x => x.Identifier == nameof(Password) && x.ErrorCode == $"{nameof(Password)}.Uppercase").ShouldBeTrue();
            errors.Any(x => x.Identifier == nameof(Password) && x.ErrorCode == $"{nameof(Password)}.Lowercase").ShouldBeTrue();
            errors.Any(x => x.Identifier == nameof(Password) && x.ErrorCode == $"{nameof(Password)}.Digit").ShouldBeTrue();
            errors.Any(x => x.Identifier == nameof(Password) && x.ErrorCode == $"{nameof(Password)}.SpecialCharacter").ShouldBeTrue();
        });
        
        errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Any(x => x.Identifier == nameof(Role) && x.ErrorCode == $"{nameof(Role)}.Required").ShouldBeTrue();
            errors.Any(x => x.Identifier == nameof(Role) && x.ErrorCode == $"{nameof(Role)}.InvalidRole").ShouldBeTrue();
        });
        
        userResult.Status.ShouldBe(ResultStatus.Invalid);
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

        A.CallTo(() => _userEfRepository.EmailExistsAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(Task.FromResult(false));

        // Act
        var result = await DomainUser.CreateAsync(builder =>
        {
            builder.FirstName = firstName;
            builder.LastName = lastName;
            builder.Email = email;
            builder.Password = password;
            builder.Role = role;
        },
        _userEfRepository);

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
        A.CallTo(() => _userEfRepository.EmailExistsAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(Task.FromResult(true));

        // Act
        var result = await DomainUser.CreateAsync(builder =>
         {
             builder.FirstName = _faker.Name.FirstName();
             builder.LastName = _faker.Name.LastName();
             builder.Email = _faker.Internet.Email();
             builder.Password = "Senha@123456";
             builder.Role = "User";
         },
        _userEfRepository);

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
        A.CallTo(() => _userEfRepository.EmailExistsAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(Task.FromResult(false));

        // Act
        var result = await DomainUser.CreateAsync(builder =>
        {
            builder.FirstName = _faker.Name.FirstName();
            builder.LastName = _faker.Name.LastName();
            builder.Email = _faker.Internet.Email();
            builder.Password = invalidPassword;
            builder.Role = "User";
        },
        _userEfRepository);

        // Assert
        result.Status.ShouldBe(ResultStatus.Invalid);
        result.ValidationErrors.ShouldContain(x => x.Identifier == nameof(Password));
    }

    [Fact]
    public async Task Create_User_Should_Return_Error_When_All_Child_Fields_Are_Valid_But_Root_Class_Not()
    {
        // Arrange
        var email = _faker.Internet.Email();
        var password = "Senha@123456";
        var role = "User";

        A.CallTo(() => _userEfRepository.EmailExistsAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(Task.FromResult(false));

        // Act
        var result = await DomainUser.CreateAsync(builder =>
        {
            builder.FirstName = string.Empty;
            builder.LastName = string.Empty;
            builder.Email = email;
            builder.Password = password;
            builder.Role = role;
        },
        _userEfRepository);

        // Assert
        result.Status.ShouldBe(ResultStatus.Invalid);
        result.Value.ShouldBeNull();
    }

    [Fact]
    public async Task Create_User_Should_Return_Invalid_When_String_Fields_Exceeds_Max_Length()
    {
        // Arrange
        var firstName = _faker.Name.FirstName().PadRight(201, 'a'); // 201 characters
        var lastName = _faker.Name.LastName().PadRight(201, 'a'); // 201 characters
        var email = _faker.Internet.Email().PadRight(201, 'a'); // 201 characters
        var password = _faker.Internet.Password(201); // 201 characters
        var role = _faker.Lorem.Word().PadRight(21, 'a'); // 21 characters

        A.CallTo(() => _userEfRepository.EmailExistsAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(Task.FromResult(false));
        // Act
        var result = await DomainUser.CreateAsync(builder =>
        {
            builder.FirstName = firstName;
            builder.LastName = lastName;
            builder.Email = email;
            builder.Password = password;
            builder.Role = role;
        },
        _userEfRepository);
        
        // Assert
        result.Status.ShouldBe(ResultStatus.Invalid);
        //result.ValidationErrors.ShouldContain(x => x.Identifier == nameof(DomainUser.FirstName));
        //result.ValidationErrors.ShouldContain(x => x.Identifier == nameof(DomainUser.LastName));
        result.ValidationErrors.ShouldContain(x => x.Identifier == nameof(Email));
        result.ValidationErrors.ShouldContain(x => x.Identifier == nameof(Password));
        result.ValidationErrors.ShouldContain(x => x.Identifier == nameof(Role));
    }

    private static IEnumerable<(string Identifier, int Count, IEnumerable<string> ErrorCodes)> GroupValidationErrorsByIdentifier(IEnumerable<ValidationError> errors)
    {
        return errors
            .GroupBy(e => e.Identifier)
            .Select(g => (
                Identifier: g.Key,
                Count: g.Count(),
                ErrorCodes: g.Select(e => $"{e.ErrorCode} - {e.ErrorMessage}")
            ));
    }
}