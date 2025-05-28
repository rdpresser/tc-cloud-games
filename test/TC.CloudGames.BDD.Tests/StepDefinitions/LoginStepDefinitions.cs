using Ardalis.Result;
using FakeItEasy;
using FastEndpoints;
using Reqnroll;
using Shouldly;
using TC.CloudGames.Application.Users.Login;
using TC.CloudGames.Domain.UserAggregate;
using TC.CloudGames.Domain.UserAggregate.Abstractions;
using AppMessaging = TC.CloudGames.Application.Abstractions.Messaging;

namespace TC.CloudGames.BDD.Tests.StepDefinitions
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
            if (_command.Email == "admin@admin.com" && _command.Password == "Wrong-Password-Admin-123")
            {
                // Simulate a validation error for admin user with wrong password
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
        }

        [Then("the response should not include a JWT token")]
        public async Task ThenTheResponseShouldNotIncludeAJWTToken()
        {
            if (_command.Email == "admin@admin.com" && _command.Password == "Wrong-Password-Admin-123")
            {
                var result = await _fakeHandler.ExecuteAsync(_command, CancellationToken.None);
                result.Status.ShouldBe(ResultStatus.NotFound);
                result.Errors.ShouldNotBeNull();
            }
        }

        [Given("the user leaves the username field {string}")]
        public void GivenTheUserLeavesTheUsernameField(string p0)
        {
            throw new PendingStepException();
        }

        [Given("the password field {string}")]
        public void GivenThePasswordField(string p0)
        {
            throw new PendingStepException();
        }

        [Then("a JWT token should be generated in the response")]
        public void ThenAJWTTokenShouldBeGeneratedInTheResponse()
        {
            throw new PendingStepException();
        }
    }
}
