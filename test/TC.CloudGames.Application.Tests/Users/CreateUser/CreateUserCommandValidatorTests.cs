using Bogus;
using FakeItEasy;
using FluentValidation.TestHelper;
using TC.CloudGames.Application.Abstractions.Data;
using TC.CloudGames.Application.Users.CreateUser;

namespace TC.CloudGames.Application.Tests.Users.CreateUser
{
    public class CreateUserCommandValidatorTests
    {
        private readonly Faker _faker;
        private readonly IUserPgRepository _userPgRepository;
        private readonly CreateUserCommandValidator _validator;

        public CreateUserCommandValidatorTests()
        {
            _faker = new Faker();
            _userPgRepository = A.Fake<IUserPgRepository>();
            _validator = new CreateUserCommandValidator(_userPgRepository);
        }

        [Fact]
        public async Task Should_Have_Error_When_FirstName_Is_Empty()
        {
            var command = new CreateUserCommand(
                FirstName: null,
                LastName: _faker.Name.LastName(),
                Email: _faker.Internet.Email(),
                Password: _faker.Internet.Password(),
                Role: "User"
            );

            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.FirstName);
        }

        [Fact]
        public async Task Should_Have_Error_When_LastName_Is_Empty()
        {
            var command = new CreateUserCommand(
                FirstName: _faker.Name.FirstName(),
                LastName: null,
                Email: _faker.Internet.Email(),
                Password: _faker.Internet.Password(),
                Role: "User"
            );

            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.LastName);
        }

        [Fact]
        public async Task Should_Have_Error_When_Email_Is_Empty()
        {
            var command = new CreateUserCommand(
                FirstName: _faker.Name.FirstName(),
                LastName: _faker.Name.LastName(),
                Email: null,
                Password: _faker.Internet.Password(),
                Role: "User"
            );

            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public async Task Should_Have_Error_When_Email_Already_Exists()
        {
            var command = new CreateUserCommand(
                FirstName: _faker.Name.FirstName(),
                LastName: _faker.Name.LastName(),
                Email: _faker.Internet.Email(),
                Password: _faker.Internet.Password(),
                Role: "User"
            );

            A.CallTo(() => _userPgRepository.EmailExistsAsync(command.Email, A<CancellationToken>._))
                .Returns(Task.FromResult(true));

            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.Email)
                .WithErrorCode($"{nameof(CreateUserCommand.Email)}.AlreadyExists");
        }

        [Fact]
        public async Task Should_Have_Error_When_Role_Is_Empty()
        {
            var command = new CreateUserCommand(
                FirstName: _faker.Name.FirstName(),
                LastName: _faker.Name.LastName(),
                Email: _faker.Internet.Email(),
                Password: _faker.Internet.Password(),
                Role: null
            );

            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.Role);
        }

        [Fact]
        public async Task Should_Have_Error_When_Password_Is_Empty()
        {
            var command = new CreateUserCommand(
                FirstName: _faker.Name.FirstName(),
                LastName: _faker.Name.LastName(),
                Email: _faker.Internet.Email(),
                Password: null,
                Role: "User"
            );

            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Fact]
        public async Task Should_Not_Have_Error_When_Command_Is_Valid()
        {
            var command = new CreateUserCommand(
                FirstName: _faker.Name.FirstName(),
                LastName: _faker.Name.LastName(),
                Email: _faker.Internet.Email(),
                Password: _faker.Internet.Password() + "!",
                Role: "User"
            );

            A.CallTo(() => _userPgRepository.EmailExistsAsync(command.Email, A<CancellationToken>._))
                .Returns(Task.FromResult(false));

            var result = await _validator.TestValidateAsync(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}