using Microsoft.AspNetCore.Http;
using TC.CloudGames.Api.Endpoints.Game;
using TC.CloudGames.Application.Abstractions;
using TC.CloudGames.Application.Games.GetGameById;
using TC.CloudGames.Domain.Aggregates.Game.Abstractions;
using TC.CloudGames.Unit.Tests.Api.Abstractions;

namespace TC.CloudGames.Unit.Tests.Api.Endpoints.Game
{
    public class GetGameByIdEndpointTests(App App) : TestBase<App>
    {
        [Fact]
        public async Task GetGameById_ValidId_ReturnsGame()
        {
            // Arrange
            var httpContext = App.GetValidUserContextAccessor();
            var userContext = App.GetValidLoggedUser();
            var cache = App.GetCache();

            var ep = Factory.Create<GetGameByIdEndpoint>((httpContext.HttpContext as DefaultHttpContext)!, cache, userContext);
            var getGameReq = new GetGameByIdQuery(Id: userContext.UserId);
            var getGameRes = GameByIdResponse.Create(builder =>
            {
                builder.Id = userContext.UserId;
                builder.Name = "Test Game";
                builder.ReleaseDate = DateOnly.FromDateTime(DateTime.UtcNow);
                builder.AgeRating = "PG-13";
                builder.Description = "Test Description";
                builder.DiskSize = 50.0m;
                builder.Price = 29.99m;
                builder.Rating = 4.5m;
                builder.OfficialLink = "https://example.com/game";
                builder.DeveloperInfo = new("Test Developer", null);
                builder.GameDetails = new("Test Genre", ["PC"], null, "Test Platform", "Test Language", null, false);
                builder.SystemRequirements = new("Test OS", null);
                builder.Playtime = new(null, null);
                builder.GameStatus = "Available";
                builder.OfficialLink = "https://example.com/game";
            });

            var fakeHandler = A.Fake<QueryHandler<GetGameByIdQuery, GameByIdResponse>>();
            A.CallTo(() => fakeHandler.ExecuteAsync(A<GetGameByIdQuery>.Ignored, A<CancellationToken>.Ignored))
                .Returns(Task.FromResult(Result<GameByIdResponse>.Success(getGameRes)));

            fakeHandler.RegisterForTesting();

            // Act
            await ep.HandleAsync(getGameReq, TestContext.Current.CancellationToken);

            // Assert
            ep.Response.Id.ShouldBe(getGameRes.Id);
            ep.Response.Name.ShouldBe(getGameRes.Name);
            ep.Response.AgeRating.ShouldBe(getGameRes.AgeRating);
            ep.Response.GameStatus.ShouldBe(getGameRes.GameStatus);
            ep.Response.GameDetails.ShouldBe(getGameRes.GameDetails);

            // Additional Assertions
            var result = await fakeHandler.ExecuteAsync(getGameReq, CancellationToken.None);
            result.IsSuccess.ShouldBeTrue();
            result.IsInvalid().ShouldBeFalse();
            result.ValidationErrors.Count().ShouldBe(0);
            result.Value.ShouldNotBeNull();
            result.Errors.Count().ShouldBe(0);
        }

        [Fact]
        public async Task GetGameById_ValidId_ReturnsNotFound()
        {
            // Arrange
            var httpContext = App.GetValidUserContextAccessor(AppConstants.UserRole);
            var userContext = App.GetValidLoggedUser(AppConstants.UserRole);
            var cache = App.GetCache();

            var ep = Factory.Create<GetGameByIdEndpoint>((httpContext.HttpContext as DefaultHttpContext)!, cache, userContext);
            var getGameReq = new GetGameByIdQuery(Id: userContext.UserId);

            var listError = new List<ValidationError>
            {
                new()
                {
                    Identifier = "Id",
                    ErrorMessage = $"Game with id '{getGameReq.Id}' not found.",
                    ErrorCode = GameDomainErrors.NotFound.ErrorCode,
                    Severity = ValidationSeverity.Error
                }
            };

            var fakeHandler = A.Fake<QueryHandler<GetGameByIdQuery, GameByIdResponse>>();
            A.CallTo(() => fakeHandler.ExecuteAsync(A<GetGameByIdQuery>.Ignored, A<CancellationToken>.Ignored))
                .Returns(Task.FromResult(Result<GameByIdResponse>.NotFound([.. listError.Select(x => x.ErrorMessage)])));

            fakeHandler.RegisterForTesting();

            // Act
            await ep.HandleAsync(getGameReq, TestContext.Current.CancellationToken);

            // Assert
            ep.Response.Id.ShouldBe(Guid.Empty);
            ep.Response.Name.ShouldBeNull();
            ep.Response.AgeRating.ShouldBeNull();
            ep.Response.GameStatus.ShouldBeNull();
            ep.Response.GameDetails.ShouldBeNull();

            // Additional Assertions
            var result = await fakeHandler.ExecuteAsync(getGameReq, CancellationToken.None);
            result.IsSuccess.ShouldBeFalse();
            result.IsInvalid().ShouldBeFalse();
            result.IsNotFound().ShouldBeTrue();
            result.ValidationErrors.Count().ShouldBe(0);
            result.Value.ShouldBeNull();
            result.Errors.Count().ShouldBe(1);
        }

        [Fact]
        public async Task GetGameById_ValidId_ReturnsUnauthorized()
        {
            // Arrange
            var httpContext = App.GetValidUserContextAccessor(AppConstants.UserRole);
            var userContext = App.GetValidLoggedUser(AppConstants.UserRole);
            var cache = App.GetCache();

            var ep = Factory.Create<GetGameByIdEndpoint>((httpContext.HttpContext as DefaultHttpContext)!, cache, userContext);
            var getGameReq = new GetGameByIdQuery(Id: userContext.UserId);

            var listError = new List<ValidationError>
            {
                new()
                {
                    Identifier = "Id",
                    ErrorMessage = "You are not authorized to access this game.",
                    ErrorCode = $"Id.NotAuthorized",
                    Severity = ValidationSeverity.Error
                }
            };

            var fakeHandler = A.Fake<QueryHandler<GetGameByIdQuery, GameByIdResponse>>();
            A.CallTo(() => fakeHandler.ExecuteAsync(A<GetGameByIdQuery>.Ignored, A<CancellationToken>.Ignored))
                .Returns(Task.FromResult(Result<GameByIdResponse>.Unauthorized([.. listError.Select(x => x.ErrorMessage)])));

            fakeHandler.RegisterForTesting();

            // Act
            await ep.HandleAsync(getGameReq, TestContext.Current.CancellationToken);

            // Assert
            ep.Response.Id.ShouldBe(Guid.Empty);
            ep.Response.Name.ShouldBeNull();
            ep.Response.AgeRating.ShouldBeNull();
            ep.Response.GameStatus.ShouldBeNull();
            ep.Response.GameDetails.ShouldBeNull();

            // Additional Assertions
            var result = await fakeHandler.ExecuteAsync(getGameReq, CancellationToken.None);
            result.IsSuccess.ShouldBeFalse();
            result.IsInvalid().ShouldBeFalse();
            result.IsNotFound().ShouldBeFalse();
            result.IsUnauthorized().ShouldBeTrue();
            result.ValidationErrors.Count().ShouldBe(0);
            result.Value.ShouldBeNull();
            result.Errors.Count().ShouldBe(1);
        }

        [Fact]
        public async Task GetGameById_ValidEmail_ReturnsForbidden()
        {
            // Arrange
            var httpContext = App.GetValidUserContextAccessor(AppConstants.UnknownRole);
            var userContext = App.GetValidLoggedUser(AppConstants.UnknownRole);
            var cache = App.GetCache();

            var ep = Factory.Create<GetGameByIdEndpoint>((httpContext.HttpContext as DefaultHttpContext)!, cache, userContext);
            var getGameReq = new GetGameByIdQuery(Id: userContext.UserId);

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

            var fakeHandler = A.Fake<QueryHandler<GetGameByIdQuery, GameByIdResponse>>();
            A.CallTo(() => fakeHandler.ExecuteAsync(A<GetGameByIdQuery>.Ignored, A<CancellationToken>.Ignored))
                .Returns(Task.FromResult(Result<GameByIdResponse>.Forbidden([.. listError.Select(x => x.ErrorMessage)])));

            fakeHandler.RegisterForTesting();

            // Act
            await ep.HandleAsync(getGameReq, TestContext.Current.CancellationToken);

            // Assert
            ep.Response.Id.ShouldBe(Guid.Empty);
            ep.Response.Name.ShouldBeNull();
            ep.Response.AgeRating.ShouldBeNull();
            ep.Response.GameStatus.ShouldBeNull();
            ep.Response.GameDetails.ShouldBeNull();

            // Additional Assertions
            var result = await fakeHandler.ExecuteAsync(getGameReq, CancellationToken.None);
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
