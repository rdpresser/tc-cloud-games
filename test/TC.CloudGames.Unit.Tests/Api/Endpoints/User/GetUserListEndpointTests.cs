using Microsoft.AspNetCore.Http;
using TC.CloudGames.Api.Endpoints.User;
using TC.CloudGames.Api.Tests.Abstractions;
using TC.CloudGames.Application.Abstractions;
using TC.CloudGames.Application.Users.GetUserList;

namespace TC.CloudGames.Api.Tests.Endpoints.User
{
    public class GetUserListEndpointTests(App App) : TestBase<App>
    {
        [Fact]
        public async Task GetUserList_ValidFilters_ReturnsUserList()
        {
            // Arrange
            var httpContext = App.GetValidUserContextAccessor(AppConstants.UserRole);
            var userContext = App.GetValidLoggedUser(AppConstants.UserRole);
            var cache = App.GetCache();

            var ep = Factory.Create<GetUserListEndpoint>((httpContext.HttpContext as DefaultHttpContext)!, cache, userContext);
            var getUserReq = new GetUserListQuery(PageNumber: 1, PageSize: 20, SortBy: "FirstName", SortDirection: "ASC", Filter: "");

            var getUserRes = new List<UserListResponse>
            {
                new()
                {
                    Id = userContext.UserId,
                    FirstName = "Regular",
                    LastName = "User",
                    Email = userContext.UserEmail,
                    Role = "User"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Admin",
                    LastName = "User",
                    Email = "admin@admin.com",
                    Role = "Admin"
                }
            };

            var fakeHandler = A.Fake<IAppCommandHandler.QueryHandler<GetUserListQuery, IReadOnlyList<UserListResponse>>>();
            A.CallTo(() => fakeHandler.ExecuteAsync(A<GetUserListQuery>.Ignored, A<CancellationToken>.Ignored))
                .Returns(Task.FromResult(Result<IReadOnlyList<UserListResponse>>.Success(getUserRes)));

            fakeHandler.RegisterForTesting();

            // Act
            await ep.HandleAsync(getUserReq, TestContext.Current.CancellationToken);

            // Assert
            ep.Response[0].Id.ShouldBe(getUserRes[0].Id);
            ep.Response[0].FirstName.ShouldBe(getUserRes[0].FirstName);
            ep.Response[0].LastName.ShouldBe(getUserRes[0].LastName);
            ep.Response[0].Email.ShouldBe(getUserRes[0].Email);
            ep.Response[0].Role.ShouldBe(getUserRes[0].Role);

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
