using Microsoft.AspNetCore.Http;
using TC.CloudGames.Api.Endpoints.User;
using TC.CloudGames.Application.Abstractions;
using TC.CloudGames.Application.Users.GetUserByEmail;
using TC.CloudGames.Domain.UserAggregate.Abstractions;
using TC.CloudGames.Unit.Tests.Api.Abstractions;

namespace TC.CloudGames.Unit.Tests.Api.Endpoints.User
{
    public class GetUserByEmailEndpointTests(App App) : TestBase<App>
    {
        [Fact]
        public async Task GetUserByEmail_ValidEmail_ReturnsUser()
        {
            // Arrange
            var httpContext = App.GetValidUserContextAccessor();
            var userContext = App.GetValidLoggedUser();
            var cache = App.GetCache();

            var ep = Factory.Create<GetUserByEmailEndpoint>((httpContext.HttpContext as DefaultHttpContext)!, cache, userContext);
            var getUserReq = new GetUserByEmailQuery(Email: "user@user.com");
            var getUserRes = new UserByEmailResponse
            {
                Id = userContext.UserId,
                FirstName = "Regular",
                LastName = "User",
                Email = getUserReq.Email,
                Role = "User"
            };

            var fakeHandler = A.Fake<QueryHandler<GetUserByEmailQuery, UserByEmailResponse>>();
            A.CallTo(() => fakeHandler.ExecuteAsync(A<GetUserByEmailQuery>.Ignored, A<CancellationToken>.Ignored))
                .Returns(Task.FromResult(Result<UserByEmailResponse>.Success(getUserRes)));

            fakeHandler.RegisterForTesting();

            // Act
            await ep.HandleAsync(getUserReq, TestContext.Current.CancellationToken);

            // Assert
            ep.Response.Id.ShouldBe(getUserRes.Id);
            ep.Response.FirstName.ShouldBe(getUserRes.FirstName);
            ep.Response.LastName.ShouldBe(getUserRes.LastName);
            ep.Response.Email.ShouldBe(getUserRes.Email);
            ep.Response.Role.ShouldBe(getUserRes.Role);

            // Additional Assertions
            var result = await fakeHandler.ExecuteAsync(getUserReq, CancellationToken.None);
            result.IsSuccess.ShouldBeTrue();
            result.IsInvalid().ShouldBeFalse();
            result.ValidationErrors.Count().ShouldBe(0);
            result.Value.ShouldNotBeNull();
            result.Errors.Count().ShouldBe(0);
        }

        [Fact]
        public async Task GetUserByEmail_ValidEmail_ReturnsNotFound()
        {
            // Arrange
            var httpContext = App.GetValidUserContextAccessor(AppConstants.UserRole);
            var userContext = App.GetValidLoggedUser(AppConstants.UserRole);
            var cache = App.GetCache();

            var ep = Factory.Create<GetUserByEmailEndpoint>((httpContext.HttpContext as DefaultHttpContext)!, cache, userContext);
            var getUserReq = new GetUserByEmailQuery(Email: "fake.doe@test.com");

            var listError = new List<ValidationError>
            {
                new()
                {
                    Identifier = "Email",
                    ErrorMessage = $"User with email '{getUserReq.Email}' not found.",
                    ErrorCode = UserDomainErrors.NotFound.ErrorCode,
                    Severity = ValidationSeverity.Error
                }
            };

            var fakeHandler = A.Fake<QueryHandler<GetUserByEmailQuery, UserByEmailResponse>>();
            A.CallTo(() => fakeHandler.ExecuteAsync(A<GetUserByEmailQuery>.Ignored, A<CancellationToken>.Ignored))
                .Returns(Task.FromResult(Result<UserByEmailResponse>.NotFound([.. listError.Select(x => x.ErrorMessage)])));

            fakeHandler.RegisterForTesting();

            // Act
            await ep.HandleAsync(getUserReq, TestContext.Current.CancellationToken);

            // Assert
            ep.Response.Id.ShouldBe(Guid.Empty);
            ep.Response.FirstName.ShouldBeNull();
            ep.Response.LastName.ShouldBeNull();
            ep.Response.Email.ShouldBeNull();
            ep.Response.Role.ShouldBeNull();

            // Additional Assertions
            var result = await fakeHandler.ExecuteAsync(getUserReq, CancellationToken.None);
            result.IsSuccess.ShouldBeFalse();
            result.IsInvalid().ShouldBeFalse();
            result.IsNotFound().ShouldBeTrue();
            result.ValidationErrors.Count().ShouldBe(0);
            result.Value.ShouldBeNull();
            result.Errors.Count().ShouldBe(1);
        }


        [Fact]
        public async Task GetUserByEmail_ValidEmail_ReturnsUnauthorized()
        {
            // Arrange
            var httpContext = App.GetValidUserContextAccessor(AppConstants.UserRole);
            var userContext = App.GetValidLoggedUser(AppConstants.UserRole);
            var cache = App.GetCache();

            var ep = Factory.Create<GetUserByEmailEndpoint>((httpContext.HttpContext as DefaultHttpContext)!, cache, userContext);
            var getUserReq = new GetUserByEmailQuery(Email: "fake.doe@test.com");

            var listError = new List<ValidationError>
            {
                new()
                {
                    Identifier = "Email",
                    ErrorMessage = "You are not authorized to access this user.",
                    ErrorCode = $"Email.NotAuthorized",
                    Severity = ValidationSeverity.Error
                }
            };

            var fakeHandler = A.Fake<QueryHandler<GetUserByEmailQuery, UserByEmailResponse>>();
            A.CallTo(() => fakeHandler.ExecuteAsync(A<GetUserByEmailQuery>.Ignored, A<CancellationToken>.Ignored))
                .Returns(Task.FromResult(Result<UserByEmailResponse>.Unauthorized([.. listError.Select(x => x.ErrorMessage)])));

            fakeHandler.RegisterForTesting();

            // Act
            await ep.HandleAsync(getUserReq, TestContext.Current.CancellationToken);

            // Assert
            ep.Response.Id.ShouldBe(Guid.Empty);
            ep.Response.FirstName.ShouldBeNull();
            ep.Response.LastName.ShouldBeNull();
            ep.Response.Email.ShouldBeNull();
            ep.Response.Role.ShouldBeNull();

            // Additional Assertions
            var result = await fakeHandler.ExecuteAsync(getUserReq, CancellationToken.None);
            result.IsSuccess.ShouldBeFalse();
            result.IsInvalid().ShouldBeFalse();
            result.IsNotFound().ShouldBeFalse();
            result.IsUnauthorized().ShouldBeTrue();
            result.ValidationErrors.Count().ShouldBe(0);
            result.Value.ShouldBeNull();
            result.Errors.Count().ShouldBe(1);
        }

        [Fact]
        public async Task GetUserByEmail_ValidEmail_ReturnsForbidden()
        {
            // Arrange
            var httpContext = App.GetValidUserContextAccessor(AppConstants.UnknownRole);
            var userContext = App.GetValidLoggedUser(AppConstants.UnknownRole);
            var cache = App.GetCache();

            var ep = Factory.Create<GetUserByEmailEndpoint>((httpContext.HttpContext as DefaultHttpContext)!, cache, userContext);
            var getUserReq = new GetUserByEmailQuery(Email: "fake.doe@test.com");

            var listError = new List<ValidationError>
            {
                new()
                {
                    Identifier = "Email",
                    ErrorMessage = "Forbiden",
                    ErrorCode = $"Email.Forbiden",
                    Severity = ValidationSeverity.Error
                }
            };

            var fakeHandler = A.Fake<QueryHandler<GetUserByEmailQuery, UserByEmailResponse>>();
            A.CallTo(() => fakeHandler.ExecuteAsync(A<GetUserByEmailQuery>.Ignored, A<CancellationToken>.Ignored))
                .Returns(Task.FromResult(Result<UserByEmailResponse>.Forbidden([.. listError.Select(x => x.ErrorMessage)])));

            fakeHandler.RegisterForTesting();

            // Act
            await ep.HandleAsync(getUserReq, TestContext.Current.CancellationToken);

            // Assert
            ep.Response.Id.ShouldBe(Guid.Empty);
            ep.Response.FirstName.ShouldBeNull();
            ep.Response.LastName.ShouldBeNull();
            ep.Response.Email.ShouldBeNull();
            ep.Response.Role.ShouldBeNull();

            // Additional Assertions
            var result = await fakeHandler.ExecuteAsync(getUserReq, CancellationToken.None);
            result.IsSuccess.ShouldBeFalse();
            result.IsInvalid().ShouldBeFalse();
            result.IsNotFound().ShouldBeFalse();
            result.IsUnauthorized().ShouldBeFalse();
            result.IsForbidden().ShouldBeTrue();
            result.ValidationErrors.Count().ShouldBe(0);
            result.Value.ShouldBeNull();
            result.Errors.Count().ShouldBe(1);
        }
    }
}
