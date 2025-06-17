using TC.CloudGames.Domain.Aggregates.User.Abstractions;
using TC.CloudGames.Domain.Aggregates.User.ValueObjects;
using TC.CloudGames.Unit.Tests.Fakes;
using DomainUser = TC.CloudGames.Domain.Aggregates.User.User;

namespace TC.CloudGames.Unit.Tests.Domain.User;

public class UserTests
{
    private readonly IUserEfRepository _userEfRepository;

    public UserTests()
    {
        _userEfRepository = A.Fake<IUserEfRepository>();
    }

    [Fact]
    public async Task Create_User_Should_Return_Invalid_When_Required_Value_Objects_Are_Empty()
    {
        // Arrange
        A.CallTo(() => _userEfRepository.EmailExistsAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(Task.FromResult(false)); // or true, depending on your test

        var userValid = FakeUserData.UserValid();

        // Act
        var userResult = await DomainUser.CreateAsync(builder =>
        {
            builder.FirstName = userValid.FistName;
            builder.LastName = userValid.LastName;
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
        errors.Count().ShouldBe(13);

        userResult.Status.ShouldBe(ResultStatus.Invalid);
        errors.Count(x => x.Identifier == nameof(DomainUser.FirstName)).ShouldBe(0);
        errors.Count(x => x.Identifier == nameof(DomainUser.LastName)).ShouldBe(0);
        errors.Count(x => x.Identifier == nameof(Email)).ShouldBe(3);
        errors.Count(x => x.Identifier == nameof(Password)).ShouldBe(7);
        errors.Count(x => x.Identifier == nameof(Role)).ShouldBe(3);
    }

    [Fact]
    public async Task Create_User_Using_Builder_Should_Return_Invalid_When_Required_Value_Objects_Are_Empty()
    {
        // Arrange
        A.CallTo(() => _userEfRepository.EmailExistsAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(Task.FromResult(false)); // or true, depending on your test

        var userValid = FakeUserData.UserValid();
        
        var email = await Email.CreateAsync(builder => { }, _userEfRepository);
        var password = Password.Create(builder => { });
        var role = Role.Create(builder => { });

        // Act
        var userResult = DomainUser.CreateFromValueObjects(builder =>
        {
            builder.FirstName = userValid.FistName;
            builder.LastName = userValid.LastName;
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
        errors.Count(x => x.Identifier == nameof(Email)).ShouldBe(2);
        errors.Count(x => x.Identifier == nameof(Password)).ShouldBe(2);
        errors.Count(x => x.Identifier == nameof(Role)).ShouldBe(2);
    }

    [Fact]
    public async Task Create_User_Using_Builder_Result_Should_Return_Invalid_When_Required_Value_Objects_Are_Empty()
    {
        // Arrange
        A.CallTo(() => _userEfRepository.EmailExistsAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(Task.FromResult(false)); // or true, depending on your test

        var userValid = FakeUserData.UserValid();
        
        var email = await Email.CreateAsync(builder => { }, _userEfRepository);
        var password = Password.Create(builder => { });
        var role = Role.Create(builder => { });

        // Act
        var userResult = DomainUser.CreateFromResult(builder =>
        {
            builder.FirstName = userValid.FistName;
            builder.LastName = userValid.LastName;
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
        errors.Count(x => x.Identifier == nameof(Email)).ShouldBe(3);
        errors.Count(x => x.Identifier == nameof(Password)).ShouldBe(7);
        errors.Count(x => x.Identifier == nameof(Role)).ShouldBe(3);
        userResult.Status.ShouldBe(ResultStatus.Invalid);
    }

    [Fact]
    public async Task Create_User_Should_Return_Success_When_All_Fields_Are_Valid()
    {
        // Arrange
        var userValid = FakeUserData.UserValid();

        A.CallTo(() => _userEfRepository.EmailExistsAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(Task.FromResult(false));

        // Act
        var result = await DomainUser.CreateAsync(builder =>
        {
            builder.FirstName = userValid.FistName;
            builder.LastName = userValid.LastName;
            builder.Email = userValid.Email;
            builder.Password = userValid.Password;
            builder.Role = userValid.Role;
        },
        _userEfRepository);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        result.Value.ShouldNotBeNull();
        result.Value.FirstName.ShouldBe(userValid.FistName);
        result.Value.LastName.ShouldBe(userValid.LastName);
        result.Value.Email.Value.ShouldBe(userValid.Email);
    }

    [Fact]
    public async Task Create_User_Should_Return_Invalid_When_Email_Already_Exists()
    {
        // Arrange
        A.CallTo(() => _userEfRepository.EmailExistsAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(Task.FromResult(true));

        var userValid = FakeUserData.UserValid();
        
        // Act
        var result = await DomainUser.CreateAsync(builder =>
         {
             builder.FirstName = userValid.FistName;
             builder.LastName = userValid.LastName;
             builder.Email = userValid.Email;
             builder.Password = userValid.Password;
             builder.Role = userValid.Role;
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

        var userValid = FakeUserData.UserValid();
        
        // Act
        var result = await DomainUser.CreateAsync(builder =>
        {
            builder.FirstName = userValid.FistName;
            builder.LastName = userValid.LastName;
            builder.Email = userValid.Email;
            builder.Password = invalidPassword;
            builder.Role = userValid.Role;
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
        var userValid = FakeUserData.UserValid();

        A.CallTo(() => _userEfRepository.EmailExistsAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(Task.FromResult(false));

        // Act
        var result = await DomainUser.CreateAsync(builder =>
        {
            builder.FirstName = string.Empty;
            builder.LastName = string.Empty;
            builder.Email = userValid.Email;
            builder.Password = userValid.Password;
            builder.Role = userValid.Role;
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
        var firstName = new string('a', 201);
        var lastName = new string('a', 201);
        var email = new string('a', 201);
        var password = new string('a', 201);
        var role = new string('a', 201);

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
        result.ValidationErrors.ShouldContain(x => x.Identifier == nameof(DomainUser.FirstName));
        result.ValidationErrors.ShouldContain(x => x.Identifier == nameof(DomainUser.LastName));
        result.ValidationErrors.ShouldContain(x => x.Identifier == nameof(Email));
        result.ValidationErrors.ShouldContain(x => x.Identifier == nameof(Password));
        result.ValidationErrors.ShouldContain(x => x.Identifier == nameof(Role));
    }
}