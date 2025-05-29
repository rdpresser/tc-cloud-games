using Microsoft.AspNetCore.Http;
using TC.CloudGames.Api.Endpoints.User;
using TC.CloudGames.Application.Abstractions;
using TC.CloudGames.Application.Users.GetUserById;
using TC.CloudGames.Domain.UserAggregate.Abstractions;
using TC.CloudGames.Unit.Tests.Api.Abstractions;

namespace TC.CloudGames.Unit.Tests.Api.Endpoints.User
{
    public class GetUserByIdEndpointTests(App App) : TestBase<App>
    {
        [Fact]
        public async Task GetUserById_ValidId_ReturnsUser()
        {
            // Arrange
            var httpContext = App.GetValidUserContextAccessor();
            var userContext = App.GetValidLoggedUser();
            var cache = App.GetCache();

            var ep = Factory.Create<GetUserByIdEndpoint>((httpContext.HttpContext as DefaultHttpContext)!, cache, userContext);
            var getUserReq = new GetUserByIdQuery(Id: userContext.UserId);
            var getUserRes = new UserByIdResponse
            {
                Id = userContext.UserId,
                FirstName = "Regular",
                LastName = "User",
                Email = userContext.UserEmail,
                Role = "User"
            };

            var fakeHandler = A.Fake<QueryHandler<GetUserByIdQuery, UserByIdResponse>>();
            A.CallTo(() => fakeHandler.ExecuteAsync(A<GetUserByIdQuery>.Ignored, A<CancellationToken>.Ignored))
                .Returns(Task.FromResult(Result<UserByIdResponse>.Success(getUserRes)));

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
        public async Task GetUserById_ValidId_ReturnsNotFound()
        {
            // Arrange
            var httpContext = App.GetValidUserContextAccessor(AppConstants.UserRole);
            var userContext = App.GetValidLoggedUser(AppConstants.UserRole);
            var cache = App.GetCache();

            var ep = Factory.Create<GetUserByIdEndpoint>((httpContext.HttpContext as DefaultHttpContext)!, cache, userContext);
            var getUserReq = new GetUserByIdQuery(Id: userContext.UserId);

            var listError = new List<ValidationError>
            {
                new()
                {
                    Identifier = "Id",
                    ErrorMessage = $"User with id '{getUserReq.Id}' not found.",
                    ErrorCode = UserDomainErrors.NotFound.ErrorCode,
                    Severity = ValidationSeverity.Error
                }
            };

            var fakeHandler = A.Fake<QueryHandler<GetUserByIdQuery, UserByIdResponse>>();
            A.CallTo(() => fakeHandler.ExecuteAsync(A<GetUserByIdQuery>.Ignored, A<CancellationToken>.Ignored))
                .Returns(Task.FromResult(Result<UserByIdResponse>.NotFound([.. listError.Select(x => x.ErrorMessage)])));

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
        public async Task GetUserById_ValidId_ReturnsUnauthorized()
        {
            // Arrange
            var httpContext = App.GetValidUserContextAccessor(AppConstants.UserRole);
            var userContext = App.GetValidLoggedUser(AppConstants.UserRole);
            var cache = App.GetCache();

            var ep = Factory.Create<GetUserByIdEndpoint>((httpContext.HttpContext as DefaultHttpContext)!, cache, userContext);
            var getUserReq = new GetUserByIdQuery(Id: userContext.UserId);

            var listError = new List<ValidationError>
            {
                new()
                {
                    Identifier = "Id",
                    ErrorMessage = "You are not authorized to access this user.",
                    ErrorCode = $"Id.NotAuthorized",
                    Severity = ValidationSeverity.Error
                }
            };

            var fakeHandler = A.Fake<QueryHandler<GetUserByIdQuery, UserByIdResponse>>();
            A.CallTo(() => fakeHandler.ExecuteAsync(A<GetUserByIdQuery>.Ignored, A<CancellationToken>.Ignored))
                .Returns(Task.FromResult(Result<UserByIdResponse>.Unauthorized([.. listError.Select(x => x.ErrorMessage)])));

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
        public async Task GetUserById_ValidEmail_ReturnsForbidden()
        {
            // Arrange
            var httpContext = App.GetValidUserContextAccessor(AppConstants.UnknownRole);
            var userContext = App.GetValidLoggedUser(AppConstants.UnknownRole);
            var cache = App.GetCache();

            var ep = Factory.Create<GetUserByIdEndpoint>((httpContext.HttpContext as DefaultHttpContext)!, cache, userContext);
            var getUserReq = new GetUserByIdQuery(Id: userContext.UserId);

            var listError = new List<ValidationError>
            {
                new()
                {
                    Identifier = "Id",
                    ErrorMessage = "Forbiden",
                    ErrorCode = $"Id.Forbiden",
                    Severity = ValidationSeverity.Error
                }
            };

            var fakeHandler = A.Fake<QueryHandler<GetUserByIdQuery, UserByIdResponse>>();
            A.CallTo(() => fakeHandler.ExecuteAsync(A<GetUserByIdQuery>.Ignored, A<CancellationToken>.Ignored))
                .Returns(Task.FromResult(Result<UserByIdResponse>.Forbidden([.. listError.Select(x => x.ErrorMessage)])));

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
