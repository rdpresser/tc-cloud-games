using FakeItEasy;
using FluentValidation.TestHelper;
using TC.CloudGames.Application.Abstractions.Data;
using TC.CloudGames.Application.Users.CreateUser;

namespace TC.CloudGames.Application.Tests.Users.CreateUser
{
    public class CreateUserCommandValidatorTests
    {
        private readonly IUserPgRepository _userPgRepository;
        private readonly CreateUserCommandValidator _validator;

        public CreateUserCommandValidatorTests()
        {
            _userPgRepository = A.Fake<IUserPgRepository>();
            _validator = new CreateUserCommandValidator(_userPgRepository);
        }

        [Fact]
        public async Task Should_Have_Error_When_FirstName_Is_Empty()
        {
            var command = new CreateUserCommand(
                FirstName: null,
                LastName: "User",
                Email: "validuser@email.com",
                Password: "Valid@1234",
                Role: "User"
            );

            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.FirstName);
        }

        [Fact]
        public async Task Should_Have_Error_When_LastName_Is_Empty()
        {
            var command = new CreateUserCommand(
                FirstName: "John",
                LastName: null,
                Email: "validuser@email.com",
                Password: "Valid@1234",
                Role: "User"
            );

            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.LastName);
        }

        [Fact]
        public async Task Should_Have_Error_When_Email_Is_Empty()
        {
            var command = new CreateUserCommand(
                FirstName: "John",
                LastName: "User",
                Email: null,
                Password: "Valid@1234",
                Role: "User"
            );

            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public async Task Should_Have_Error_When_Email_Already_Exists()
        {
            var command = new CreateUserCommand(
                FirstName: "John",
                LastName: "User",
                Email: "validuser@email.com",
                Password: "Valid@1234",
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
                FirstName: "John",
                LastName: "User",
                Email: "validuser@email.com",
                Password: "Valid@1234",
                Role: null
            );

            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.Role);
        }

        [Fact]
        public async Task Should_Have_Error_When_Password_Is_Empty()
        {
            var command = new CreateUserCommand(
                FirstName: "John",
                LastName: "User",
                Email: "validuser@email.com",
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
                FirstName: "John",
                LastName: "User",
                Email: "validuser@email.com",
                Password: "Valid@1234",
                Role: "User"
            );

            A.CallTo(() => _userPgRepository.EmailExistsAsync(command.Email, A<CancellationToken>._))
                .Returns(Task.FromResult(false));

            var result = await _validator.TestValidateAsync(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}