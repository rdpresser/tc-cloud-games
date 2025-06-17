using FluentValidation.TestHelper;
using TC.CloudGames.Application.Users.CreateUser;
using TC.CloudGames.Unit.Tests.Fakes;

namespace TC.CloudGames.Unit.Tests.Application.Users.CreateUser
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
            var userValid = FakeUserData.UserValid();
            
            var command = new CreateUserCommand(
                FirstName: null,
                LastName: userValid.LastName,
                Email: userValid.Email,
                Password: userValid.Password,
                Role: userValid.Role
            );

            var result = await _validator.TestValidateAsync(command, null, TestContext.Current.CancellationToken);
            result.ShouldHaveValidationErrorFor(x => x.FirstName);
        }

        [Fact]
        public async Task Should_Have_Error_When_LastName_Is_Empty()
        {
            var userValid = FakeUserData.UserValid();
            
            var command = new CreateUserCommand(
                FirstName: userValid.FistName,
                LastName: null,
                Email: userValid.Email,
                Password: userValid.Password,
                Role: userValid.Role
            );

            var result = await _validator.TestValidateAsync(command, null, TestContext.Current.CancellationToken);
            result.ShouldHaveValidationErrorFor(x => x.LastName);
        }

        [Fact]
        public async Task Should_Have_Error_When_Email_Is_Empty()
        {
            var userValid = FakeUserData.UserValid();
            
            var command = new CreateUserCommand(
                FirstName: userValid.FistName,
                LastName: userValid.LastName,
                Email: null,
                Password: userValid.Password,
                Role: userValid.Role
            );

            var result = await _validator.TestValidateAsync(command, null, TestContext.Current.CancellationToken);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public async Task Should_Have_Error_When_Email_Already_Exists()
        {
            var userValid = FakeUserData.UserValid();
            
            var command = new CreateUserCommand(
                FirstName: userValid.FistName,
                LastName: userValid.LastName,
                Email: userValid.Email,
                Password: userValid.Password,
                Role: userValid.Role
            );

            A.CallTo(() => _userPgRepository.EmailExistsAsync(command.Email, A<CancellationToken>._))
                .Returns(Task.FromResult(true));

            var result = await _validator.TestValidateAsync(command, null, TestContext.Current.CancellationToken);
            result.ShouldHaveValidationErrorFor(x => x.Email)
                .WithErrorCode($"{nameof(CreateUserCommand.Email)}.AlreadyExists");
        }

        [Fact]
        public async Task Should_Have_Error_When_Role_Is_Empty()
        {
            var userValid = FakeUserData.UserValid();
            
            var command = new CreateUserCommand(
                FirstName: userValid.FistName,
                LastName: userValid.LastName,
                Email: userValid.Email,
                Password: userValid.Password,
                Role: null
            );

            var result = await _validator.TestValidateAsync(command, null, TestContext.Current.CancellationToken);
            result.ShouldHaveValidationErrorFor(x => x.Role);
        }

        [Fact]
        public async Task Should_Have_Error_When_Password_Is_Empty()
        {
            var userValid = FakeUserData.UserValid();
            
            var command = new CreateUserCommand(
                FirstName: userValid.FistName,
                LastName: userValid.LastName,
                Email: userValid.Email,
                Password: null,
                Role: userValid.Role
            );

            var result = await _validator.TestValidateAsync(command, null, TestContext.Current.CancellationToken);
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Fact]
        public async Task Should_Not_Have_Error_When_Command_Is_Valid()
        {
            var userValid = FakeUserData.UserValid();
            
            var command = new CreateUserCommand(
                FirstName: userValid.FistName,
                LastName: userValid.LastName,
                Email: userValid.Email,
                Password: userValid.Password,
                Role: userValid.Role
            );

            A.CallTo(() => _userPgRepository.EmailExistsAsync(command.Email, A<CancellationToken>._))
                .Returns(Task.FromResult(false));

            var result = await _validator.TestValidateAsync(command, null, TestContext.Current.CancellationToken);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}