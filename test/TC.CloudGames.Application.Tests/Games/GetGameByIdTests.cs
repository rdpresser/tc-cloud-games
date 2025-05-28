using Ardalis.Result;
using Bogus;
using FakeItEasy;
using FastEndpoints;
using TC.CloudGames.Application.Abstractions.Messaging;
using TC.CloudGames.Application.Games.GetGameById;
using TC.CloudGames.Domain.GameAggregate.ValueObjects;
using DeveloperInfo = TC.CloudGames.Application.Games.GetGameById.DeveloperInfo;
using GameDetails = TC.CloudGames.Domain.GameAggregate.ValueObjects.GameDetails;
using Playtime = TC.CloudGames.Application.Games.GetGameById.Playtime;
using Price = TC.CloudGames.Application.Games.GetGameById.Price;

namespace TC.CloudGames.Application.Tests.Games;

public class GetGameByIdTests
{
    private readonly Faker _faker;
    private readonly List<string> _ageRatings;

    public GetGameByIdTests()
    {
        _faker = new Faker();

        _ageRatings = [.. AgeRating.ValidRatings];
    }

    [Fact]
    public async Task GetGameById_ShouldReturnGame_WhenGameExists()
    {
        // Arrange
        Factory.RegisterTestServices(_ => { });

        string[] AvailableLanguagesList = ["English", "Spanish", "French", "German", "Japanese"];

        var name = _faker.Commerce.ProductName();
        var releaseDate = DateOnly.FromDateTime(DateTime.Now);
        var ageRating = _faker.PickRandom(_ageRatings.ToArray());
        var description = _faker.Lorem.Paragraph();
        var developerInfo = new DeveloperInfo(_faker.Company.CompanyName(), _faker.Company.CompanyName());
        var diskSize = _faker.Random.Decimal(1, 100);
        var price = _faker.Random.Decimal(10, 300);
        var playtime = new Playtime(_faker.Random.Int(1, 10), _faker.Random.Int(10, 100));

        var gameId = Guid.NewGuid();
        var getGameReq = new GetGameByIdQuery(Id: gameId);
        var expectedGame = GameByIdResponse.Create(builder =>
        {
            builder.Id = gameId;
            builder.Name = name;
            builder.ReleaseDate = releaseDate;
            builder.AgeRating = ageRating;
            builder.Description = description;
            builder.DiskSize = diskSize;
            builder.Price = price;
            builder.Rating = _faker.Random.Decimal(0, 10);
            builder.OfficialLink = _faker.Internet.Url();
            builder.DeveloperInfo = developerInfo;
            builder.GameDetails = new(
                genre: _faker.Commerce.Categories(1)[0],
                platform:
                [
                    .. _faker.PickRandom(GameDetails.ValidPlatforms,
                        _faker.Random.Int(1, GameDetails.ValidPlatforms.Count))
                ],
                tags: string.Join(", ", _faker.Lorem.Words(5)),
                gameMode: _faker.PickRandom(GameDetails.ValidGameModes.ToArray()),
                distributionFormat: _faker.PickRandom(GameDetails.ValidDistributionFormats.ToArray()),
                availableLanguages: string.Join(", ",
                    _faker.Random.ListItems(AvailableLanguagesList,
                        _faker.Random.Int(1, AvailableLanguagesList.Length))),
                supportsDlcs: _faker.Random.Bool()
            );
            builder.SystemRequirements = new(
                minimum: _faker.Lorem.Sentence(),
                recommended: _faker.Lorem.Sentence()
            );
            builder.Playtime = new(null, null);
            builder.GameStatus = "Available";
            builder.OfficialLink = _faker.Internet.Url();
        });

        var fakeHandler = A.Fake<QueryHandler<GetGameByIdQuery, GameByIdResponse>>();
        A.CallTo(() => fakeHandler.ExecuteAsync(A<GetGameByIdQuery>.Ignored, A<CancellationToken>.Ignored))
            .Returns(Task.FromResult(Result<GameByIdResponse>.Success(expectedGame)));

        // Act
        var result = await fakeHandler.ExecuteAsync(getGameReq, CancellationToken.None);

        // Assert
        A.CallTo(() => fakeHandler.ExecuteAsync(
            A<GetGameByIdQuery>.That.Matches(q => q.Id == gameId),
            A<CancellationToken>.Ignored
        )).MustHaveHappenedOnceExactly();

        Assert.NotNull(result);
        Assert.Equal(expectedGame.Id, result.Value.Id);
    }

    [Fact]
    public void Constructor_SetsAmount()
    {
        // Arrange
        decimal expectedAmount = 59.99m;

        // Act
        var price = new Price(expectedAmount);

        // Assert
        Assert.Equal(expectedAmount, price.Amount);
    }

    [Fact]
    public void Amount_CanBeZero()
    {
        // Arrange & Act
        var price = new Price(0m);

        // Assert
        Assert.Equal(0m, price.Amount);
    }

    [Fact]
    public void Amount_CanBeNegative()
    {
        // Arrange & Act
        var price = new Price(-10m);

        // Assert
        Assert.Equal(-10m, price.Amount);
    }

}