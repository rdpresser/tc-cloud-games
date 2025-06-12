using TC.CloudGames.Api.Endpoints.Auth;
using TC.CloudGames.Application.Users.Login;
using TC.CloudGames.Domain.Aggregates.User.Abstractions;
using TC.CloudGames.Unit.Tests.Api.Abstractions;

namespace TC.CloudGames.Unit.Tests.Api.Endpoints.Auth
{
    public class LoginEndpointTests : TestBase<App>
    {
        [Fact]
        public async Task Login_ValidCredentials_ReturnsSuccess()
        {
            // Arrange
            var ep = Factory.Create<LoginEndpoint>();
            var loginReq = new LoginUserCommand(Email: "user@user.com", Password: "User@123");
            var loginRes = new LoginUserResponse("<jwt-token>", "user@user.com");

            var fakeHandler = A.Fake<CommandHandler<LoginUserCommand, LoginUserResponse, CloudGames.Domain.Aggregates.User.User, IUserEfRepository>>();
            A.CallTo(() => fakeHandler.ExecuteAsync(A<LoginUserCommand>.Ignored, A<CancellationToken>.Ignored))
                .Returns(Task.FromResult(Result<LoginUserResponse>.Success(loginRes)));

            fakeHandler.RegisterForTesting();

            // Act
            await ep.HandleAsync(loginReq, TestContext.Current.CancellationToken);

            // Assert
            ep.Response.JwtToken.ShouldBe(loginRes.JwtToken);
            ep.Response.Email.ShouldBe(loginRes.Email);

            // Additional Assertions
            var result = await fakeHandler.ExecuteAsync(loginReq, CancellationToken.None);
            result.IsSuccess.ShouldBeTrue();
            result.IsInvalid().ShouldBeFalse();
            result.ValidationErrors.Count().ShouldBe(0);
            result.Value.ShouldNotBeNull();
            result.Errors.Count().ShouldBe(0);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsFailure()
        {
            // Arrange
            var ep = Factory.Create<LoginEndpoint>();
            var loginReq = new LoginUserCommand(Email: "invaliduser", Password: "wrongpassword");
            var loginRes = new ValidationError
            {
                Identifier = UserDomainErrors.InvalidCredentials.Property,
                ErrorMessage = UserDomainErrors.InvalidCredentials.ErrorMessage,
                ErrorCode = UserDomainErrors.InvalidCredentials.ErrorCode,
                Severity = ValidationSeverity.Error
            };

            var fakeHandler = A.Fake<CommandHandler<LoginUserCommand, LoginUserResponse, CloudGames.Domain.Aggregates.User.User, IUserEfRepository>>();
            A.CallTo(() => fakeHandler.ExecuteAsync(A<LoginUserCommand>.Ignored, A<CancellationToken>.Ignored))
                .Returns(Task.FromResult(Result<LoginUserResponse>.Invalid(loginRes)));

            fakeHandler.RegisterForTesting();

            // Act
            await ep.HandleAsync(loginReq, TestContext.Current.CancellationToken);

            // Assert
            ep.Response.JwtToken.ShouldBe(null);
            ep.Response.Email.ShouldBe(null);

            // Additional Assertions
            var result = await fakeHandler.ExecuteAsync(loginReq, CancellationToken.None);
            result.IsSuccess.ShouldBeFalse();
            result.IsInvalid().ShouldBeTrue();
            result.ValidationErrors.ToArray()[0].ShouldBeOfType<ValidationError>();
            result.ValidationErrors.ToArray()[0].ErrorMessage.ShouldBe(UserDomainErrors.InvalidCredentials.ErrorMessage);
            result.ValidationErrors.ToArray()[0].Identifier.ShouldBe(UserDomainErrors.InvalidCredentials.Property);
            result.ValidationErrors.ToArray()[0].ErrorCode.ShouldBe(UserDomainErrors.InvalidCredentials.ErrorCode);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsNotFound()
        {
            // Arrange
            var ep = Factory.Create<LoginEndpoint>();
            var loginReq = new LoginUserCommand(Email: "fake@user.com", Password: "fakePassword");
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

            var fakeHandler = A.Fake<CommandHandler<LoginUserCommand, LoginUserResponse, CloudGames.Domain.Aggregates.User.User, IUserEfRepository>>();
            A.CallTo(() => fakeHandler.ExecuteAsync(A<LoginUserCommand>.Ignored, A<CancellationToken>.Ignored))
                .Returns(Task.FromResult(Result<LoginUserResponse>.NotFound([.. listError.Select(x => x.ErrorMessage)])));

            fakeHandler.RegisterForTesting();

            // Act
            await ep.HandleAsync(loginReq, TestContext.Current.CancellationToken);

            // Assert
            ep.Response.JwtToken.ShouldBe(null);
            ep.Response.Email.ShouldBe(null);

            // Additional Assertions
            var result = await fakeHandler.ExecuteAsync(loginReq, CancellationToken.None);
            result.IsSuccess.ShouldBeFalse();
            result.IsNotFound().ShouldBeTrue();
            result.Value.ShouldBeNull();
            result.Errors.ToArray()[0].ShouldBeOfType<string>();
            result.Errors.ToArray()[0].ShouldBe(UserDomainErrors.InvalidCredentials.ErrorMessage);
        }
    }
}
