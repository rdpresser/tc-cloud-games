using Ardalis.Result.FluentValidation;
using TC.CloudGames.Application.Users.Login;
using TC.CloudGames.Domain.Aggregates.User;
using TC.CloudGames.Domain.Aggregates.User.Abstractions;

namespace TC.CloudGames.BDD.Tests.StepDefinitions.Auth
{
    [Binding]
    public class LoginStepDefinitions
    {
        private LoginUserCommand _command = new(string.Empty, string.Empty);
        private LoginUserResponse _response = new(string.Empty, string.Empty);

        private readonly AppMessaging.CommandHandler<LoginUserCommand, LoginUserResponse, User, IUserEfRepository> _fakeHandler;

        public LoginStepDefinitions()
        {
            Factory.RegisterTestServices(_ => { });
            _fakeHandler = A.Fake<AppMessaging.CommandHandler<LoginUserCommand, LoginUserResponse, User, IUserEfRepository>>();
        }

        [Given("the user enters the username {string}")]
        public void GivenTheUserEntersTheUsername(string p0)
        {
            _command = _command with { Email = p0 };
        }

        [Given("the password {string}")]
        public void GivenThePassword(string p0)
        {
            _command = _command with { Password = p0 };
        }

        [When("they submit the login request")]
        public void WhenTheySubmitTheLoginRequest()
        {
            if (IsAdminWithWrongPassword() ||
                IsUserWithWrongPassword() ||
                IsFakeUserWithFakePassword())
            {
                var listError = new List<ValidationError>
                {
                    new()
                    {
                        Identifier = UserDomainErrors.InvalidCredentials.Property,
                        ErrorMessage = UserDomainErrors.InvalidCredentials.ErrorMessage,
                        ErrorCode = UserDomainErrors.InvalidCredentials.ErrorCode,
                        Severity = ValidationSeverity.Error
                    }
                };

                A.CallTo(() => _fakeHandler.ExecuteAsync(A<LoginUserCommand>.Ignored, A<CancellationToken>.Ignored))
                    .Returns(Result<LoginUserResponse>.NotFound([.. listError.Select(x => x.ErrorMessage)]));
            }
            else if (IsEmptyUserWithEmptyPassword())
            {
                var listError = new LoginUserCommandValidator()
                    .Validate(_command)
                    .AsErrors();

                A.CallTo(() => _fakeHandler.ExecuteAsync(A<LoginUserCommand>.Ignored, A<CancellationToken>.Ignored))
                    .Returns(Result<LoginUserResponse>.Invalid(listError));
            }
            else if (IsAdminWithCorrectPassword() || IsUserWithCorrectPassword())
            {
                _response = _response with { Email = _command.Email, JwtToken = "Fake-JWT-Token" };
                A.CallTo(() => _fakeHandler.ExecuteAsync(A<LoginUserCommand>.Ignored, A<CancellationToken>.Ignored))
                    .Returns(Result<LoginUserResponse>.Success(_response));
            }
        }

        private bool IsAdminWithCorrectPassword()
        {
            return _command.Email == "admin@admin.com" && _command.Password == "Admin@123";
        }

        private bool IsUserWithCorrectPassword()
        {
            return _command.Email == "user@user.com" && _command.Password == "User@123";
        }

        private bool IsAdminWithWrongPassword()
        {
            return _command.Email == "admin@admin.com" && _command.Password == "Wrong-Password-Admin-123";
        }

        private bool IsUserWithWrongPassword()
        {
            return _command.Email == "user@user.com" && _command.Password == "Wrong-Password-User-123";
        }

        private bool IsFakeUserWithFakePassword()
        {
            return _command.Email == "fake@User.com" && _command.Password == "Fake-Password-123";
        }

        private bool IsEmptyUserWithEmptyPassword()
        {
            return _command.Email == string.Empty && _command.Password == string.Empty;
        }

        [Then("the response should not include a JWT token")]
        public async Task ThenTheResponseShouldNotIncludeAJWTToken()
        {
            var result = await _fakeHandler.ExecuteAsync(_command, CancellationToken.None);

            if (IsAdminWithWrongPassword() ||
                IsUserWithWrongPassword() ||
                IsFakeUserWithFakePassword())
            {

                result.Status.ShouldBe(ResultStatus.NotFound);
                result.Errors.ShouldNotBeNull();
            }
            else if (IsEmptyUserWithEmptyPassword())
            {
                result.Status.ShouldBe(ResultStatus.Invalid);
                result.ValidationErrors.ShouldNotBeNull();
                result.ValidationErrors.Count().ShouldBe(8);
            }
        }

        [Given("the user leaves the username field {string}")]
        public void GivenTheUserLeavesTheUsernameField(string p0)
        {
            _command = _command with { Email = p0 };
        }

        [Given("the password field {string}")]
        public void GivenThePasswordField(string p0)
        {
            _command = _command with { Password = p0 };
        }

        [Then("a JWT token should be generated in the response")]
        public async Task ThenAJWTTokenShouldBeGeneratedInTheResponse()
        {
            var result = await _fakeHandler.ExecuteAsync(_command, CancellationToken.None);

            result.Status.ShouldBe(ResultStatus.Ok);
            result.IsSuccess.ShouldBeTrue();
        }
    }
}
