using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;
using TC.CloudGames.Api.Endpoints.User;
using TC.CloudGames.Application.Users.GetUserByEmail;
using TC.CloudGames.Infra.CrossCutting.Commons.Authentication;
using ZiggyCreatures.Caching.Fusion;

namespace TC.CloudGames.Api.Tests.Endpoints.User
{
    public class GetUserByEmailEndpointTests(App App) : TestBase<App>
    {
        //[Fact]
        //public async Task GetUserByEmail_ValidEmail_ReturnsUser3()
        //{
        //    // Arrange
        //    //var userContext = App.Services.GetKeyedService<IUserContext>("ValidLoggedUser");
        //    var userContext = App.Services.GetRequiredService<IUserContext>();
        //    var cache = App.Services.GetRequiredService<IFusionCache>();

        //    var ep = Factory.Create<GetUserByEmailEndpoint>(cache, userContext);
        //    var getUserReq = new GetUserByEmailQuery(Email: "john.doe@test.com");
        //    var getUserRes = new UserByEmailResponse
        //    {
        //        Id = Guid.NewGuid(),
        //        FirstName = "John",
        //        LastName = "Doe",
        //        Email = getUserReq.Email,
        //        Role = "User"
        //    };

        //    var fakeHandler = A.Fake<IAppCommandHandler.QueryHandler<GetUserByEmailQuery, UserByEmailResponse>>();
        //    A.CallTo(() => fakeHandler.ExecuteAsync(A<GetUserByEmailQuery>.Ignored, A<CancellationToken>.Ignored))
        //        .Returns(Task.FromResult(Result<UserByEmailResponse>.Success(getUserRes)));

        //    fakeHandler.RegisterForTesting();

        //    // Act
        //    await ep.HandleAsync(getUserReq, TestContext.Current.CancellationToken);

        //    // Assert
        //    ep.Response.Id.ShouldBe(getUserRes.Id);
        //    ep.Response.FirstName.ShouldBe(getUserRes.FirstName);
        //    ep.Response.LastName.ShouldBe(getUserRes.LastName);
        //    ep.Response.Email.ShouldBe(getUserRes.Email);
        //    ep.Response.Role.ShouldBe(getUserRes.Role);

        //    // Additional Assertions
        //    var result = await fakeHandler.ExecuteAsync(getUserReq, CancellationToken.None);
        //    result.IsSuccess.ShouldBeTrue();
        //    result.IsInvalid().ShouldBeFalse();
        //    result.ValidationErrors.Count().ShouldBe(0);
        //    result.Value.ShouldNotBeNull();
        //    result.Errors.Count().ShouldBe(0);
        //}

        [Fact]
        public async Task GetUserByEmail_ValidEmail_ReturnsUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new(JwtRegisteredClaimNames.Email, "john.doe@test.com"),
                new(JwtRegisteredClaimNames.Name, "John Doe"),
                new("role", "User")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var httpContext = new DefaultHttpContext
            {
                User = claimsPrincipal
            };

            var httpContextAccessor = A.Fake<IHttpContextAccessor>();
            A.CallTo(() => httpContextAccessor.HttpContext).Returns(httpContext);

            var userContext = new UserContext(httpContextAccessor);

            var cache = App.Services.GetRequiredService<IFusionCache>();

            var ep = Factory.Create<GetUserByEmailEndpoint>(cache, userContext);
            var getUserReq = new GetUserByEmailQuery(Email: "john.doe@test.com");
            var getUserRes = new UserByEmailResponse
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Email = getUserReq.Email,
                Role = "User"
            };

            var fakeHandler = A.Fake<IAppCommandHandler.QueryHandler<GetUserByEmailQuery, UserByEmailResponse>>();
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
    }
}
