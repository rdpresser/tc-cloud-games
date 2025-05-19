using Ardalis.Result;
using FakeItEasy;
using FastEndpoints;
using NSubstitute;
using Shouldly;
using TC.CloudGames.Api.Endpoints.Auth;
using TC.CloudGames.Application.Users.Login;
using TC.CloudGames.Domain.User.Abstractions;
using IAppCommandHandler = TC.CloudGames.Application.Abstractions.Messaging;

namespace TC.CloudGames.Api.Tests.Endpoints.Auth
{
    public class LoginEndpointTests(App App) : TestBase<App>
    {
        [Fact]
        public async Task Login_ValidCredentials_ReturnsSuccess()
        {
            // Arrange
            var ep = Factory.Create<LoginEndpoint>();
            var loginReq = new LoginUserCommand(Email: "user@user.com", Password: "User@123");
            var loginRes = new LoginUserResponse("<jwt-token>", "user@user.com");

            var fakeHandler = A.Fake<IAppCommandHandler.CommandHandler<LoginUserCommand, LoginUserResponse, Domain.User.User, IUserEfRepository>>();
            A.CallTo(() => fakeHandler.ExecuteAsync(A<LoginUserCommand>.Ignored, A<CancellationToken>.Ignored))
                .Returns(Task.FromResult(Result<LoginUserResponse>.Success(loginRes)));

            fakeHandler.RegisterForTesting();

            // Act
            await ep.HandleAsync(loginReq, TestContext.Current.CancellationToken);

            // Assert
            ep.Response.JwtToken.ShouldBe(loginRes.JwtToken);
            ep.Response.Email.ShouldBe(loginRes.Email);
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

            var fakeHandler = A.Fake<IAppCommandHandler.CommandHandler<LoginUserCommand, LoginUserResponse, Domain.User.User, IUserEfRepository>>();
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

            var fakeHandler = A.Fake<IAppCommandHandler.CommandHandler<LoginUserCommand, LoginUserResponse, Domain.User.User, IUserEfRepository>>();
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

        /*
         * Save the code below when creating integration testing, because this code calls the full integration testing
         */
        //[Fact]
        //public async Task Login_ValidCredentials_ReturnsSuccess_2()
        //{
        //    // Arrange
        //    var loginRes = new LoginUserResponse("<jwt-toke>", "user@user.com");
        //    var loginReq = new LoginUserCommand(Email: "user@user.com", Password: "User@123");

        //    // Act
        //    var (rsp, res) = await App.Client
        //        .POSTAsync<LoginEndpoint, LoginUserCommand, LoginUserResponse>(loginReq);

        //    // Assert
        //    rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        //    res.Email.ShouldBe(loginRes.Email);
        //    res.JwtToken.ShouldBe(loginRes.JwtToken);
        //}

        //[Fact]
        //public async Task Login_InvalidCredentials_ReturnsFailure()
        //{
        //    var (rsp, res) = await App.Client.POSTAsync<LoginEndpoint, LoginUserCommand, LoginUserResponse>(
        //                         new(Email: "invaliduser", Password: "wrongpassword"));

        //    rsp.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        //    res.ShouldBeNull();
        //}
    }
}
