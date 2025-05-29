using TC.CloudGames.Api.Endpoints.Game;
using TC.CloudGames.Unit.Tests.Api.Abstractions;

namespace TC.CloudGames.Unit.Tests.Api.Endpoints.Game
{
    public class CreateGameEndpointTests(App App) : TestBase<App>
    {
        [Fact]
        public async Task CreateGame_ValidData_ReturnsSuccess()
        {
            // Arrange
            var ep = Factory.Create<CreateGameEndpoint>();
            var createGameReq = BuildGameFixture.BuildCommand(App);
            var createGameRes = BuildGameFixture.BuildResponse(createGameReq);

            var fakeHandler = A.Fake<CommandHandler<CreateGameCommand, CreateGameResponse, CloudGames.Domain.GameAggregate.Game, IGameEfRepository>>();
            A.CallTo(() => fakeHandler.ExecuteAsync(A<CreateGameCommand>.Ignored, A<CancellationToken>.Ignored))
                .Returns(Task.FromResult(Result<CreateGameResponse>.Success(createGameRes)));

            fakeHandler.RegisterForTesting();

            // Act
            await ep.HandleAsync(createGameReq, TestContext.Current.CancellationToken);

            // Assert
            ep.Response.Id.ShouldBe(createGameRes.Id);
            ep.Response.Name.ShouldBe(createGameRes.Name);
            ep.Response.Description.ShouldBe(createGameRes.Description);
            ep.Response.GameDetails.Genre.ShouldBe(createGameRes.GameDetails.Genre);
            ep.Response.ReleaseDate.ShouldBe(createGameRes.ReleaseDate);
            ep.Response.Price.ShouldBe(createGameRes.Price);

            // Additional Assertions
            var result = await fakeHandler.ExecuteAsync(createGameReq, CancellationToken.None);
            result.IsSuccess.ShouldBeTrue();
            result.IsInvalid().ShouldBeFalse();
            result.ValidationErrors.Count().ShouldBe(0);
            result.Value.ShouldNotBeNull();
            result.Errors.Count().ShouldBe(0);
        }

        [Fact]
        public async Task CreateGame_InvalidData_ReturnsError()
        {
            // Arrange
            var ep = Factory.Create<CreateGameEndpoint>();
            var createGameReq = BuildGameFixture.BuildCommand(App);
            var updatedCommand = createGameReq with { Name = string.Empty };

            var listError = new List<ValidationError>
            {
                new() {
                    Identifier = "Name",
                    ErrorMessage = "Game name is required.",
                    ErrorCode = "Name.Required"
                }
            };

            var fakeHandler = A.Fake<CommandHandler<CreateGameCommand, CreateGameResponse, CloudGames.Domain.GameAggregate.Game, IGameEfRepository>>();
            A.CallTo(() => fakeHandler.ExecuteAsync(A<CreateGameCommand>.Ignored, A<CancellationToken>.Ignored))
                .Returns(Task.FromResult(Result<CreateGameResponse>.Invalid(listError)));
            fakeHandler.RegisterForTesting();
            // Act
            await ep.HandleAsync(updatedCommand, TestContext.Current.CancellationToken);

            // Assert
            ep.Response.Id.ShouldBe(Guid.Empty);
            ep.Response.Name.ShouldBeNull();

            // Additional Assertions
            var result = await fakeHandler.ExecuteAsync(updatedCommand, CancellationToken.None);
            result.IsSuccess.ShouldBeFalse();
            result.IsInvalid().ShouldBeTrue();
            result.ValidationErrors.Count().ShouldBe(1);
            result.Value.ShouldBeNull();
            result.Errors.Count().ShouldBe(0);
        }
    }
}
