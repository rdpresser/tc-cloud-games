using Ardalis.Result;
using Bogus;
using NSubstitute;
using Shouldly;
using TC.CloudGames.Domain.Game;
using TC.CloudGames.Domain.User;
using TC.CloudGames.Domain.User.Abstractions;

namespace TC.CloudGames.Domain.Tests;

public class GameTests
{
    private readonly Faker _faker;
    private readonly List<string> _genres;
    private readonly List<string> _platforms;
    private readonly List<string> _gameTags;
    private readonly List<string> _gameModes;
    private readonly List<string> _distributionFormats;
    private readonly List<string> _languages;
    private readonly List<string> _gameStatus;

    public GameTests()
    {
        _faker = new Faker();

        _genres = new List<string> { "Action", "Adventure", "RPG", "Strategy", "Simulation", "Racing", "Sport", "Puzzle",
            "Fighter", "Platform", "FPS", "TPS", "Survival", "Horror", "Stealth", "Open World", "MMORPG", "Roguelike",
            "Visual Novel", "Beat 'em up", "Battle Royale", "Musical", "Party Game", "Metroidvania", "Idle/Incremental",
            "Tower Defense", "MOBA", "Sandbox", "Tycoon" };

        _platforms = new List<string> { "PC", "PlayStation 4", "PlayStation 5", "Xbox One", "Xbox Series X|S", "Nintendo Switch",
            "Nintendo 3DS", "Wii U", "PlayStation Vita", "Android", "iOS", "macOS", "Linux", "Stadia", "Steam Deck", "Browser",
            "VR (Oculus Quest)", "VR (HTC Vive)", "VR (PlayStation VR)" };

        _gameTags = new List<string> { "Indie", "Multiplayer", "Singleplayer", "Co-op", "PvP", "PvE", "Online Co-op",
            "Local Multiplayer", "Story Rich", "Difficult", "Casual", "Anime", "Pixel Graphics", "Retro", "Funny", "Atmospheric",
            "Horror", "Sci-fi", "Fantasy", "Cyberpunk", "Steampunk", "Post-apocalyptic", "Choices Matter", "Narration",
            "Character Customization", "Exploration", "Loot", "Crafting", "Building", "Resource Management", "Base Building",
            "Turn-Based", "Real Time", "Fast-Paced", "Third Person", "First Person", "Top-Down", "Isometric", "Stylized",
            "Realistic", "Female Protagonist", "Controller Support", "VR Support", "Moddable", "Replay Value", "Open World",
            "Procedural Generation", "Sandbox", "Nonlinear", "Mystery", "Psychological", "Dark", "Gore", "Violent" };

        _gameModes = new List<string> { "Singleplayer", "Multiplayer", "Co-op", "PvP", "PvE", "Battle Royale", "Survival",
            "Sandbox", "Casual" };

        _distributionFormats = new List<string> { "Digital", "Physical" };

        _languages = new List<string> { "PT-BR", "EN-US", "ES-ES", "FR-FR", "ZH-CN", "JA-JP", "RU-RU", "KO-KR" };

        _gameStatus = new List<string> { "Available", "Soon", "Early Access" };
    }

    [Fact]
    public void Create_Game_Should_Return_Invalid_When_Required_Fields_Are_Empty()
    {
        // Arrange
        var result = Game.Game.Create(
            name: string.Empty,
            releaseDate: DateOnly.MinValue,
            ageRating: string.Empty,
            description: _faker.Lorem.Paragraph(),
            developerInfo: (string.Empty, _faker.Company.CompanyName()),
            diskSize: 0,
            price: 0,
            playtime: (_faker.Random.Int(1, 1000), _faker.Random.Int(1, 1000000)),
            gameDetails: (
                genre: _faker.PickRandom(_genres),
                platform: ["", string.Empty],
                tags: _faker.PickRandom(_gameTags),
                gameMode: string.Empty,
                distributionFormat: string.Empty,
                availableLanguages: _faker.PickRandom(_languages),
                supportsDlcs: _faker.Random.Bool()),
            systemRequirements: (string.Empty, _faker.Lorem.Paragraph()),
            rating: null,
            officialLink: _faker.Internet.Url(),
            gameStatus: string.Empty
        );


        // Assert
        result.Status.ShouldBe(ResultStatus.Invalid);
        var errors = result.ValidationErrors;

        errors.Count(x => x.Identifier == nameof(Game.Game.Name)).ShouldBe(0);
        errors.Count(x => x.Identifier == nameof(Game.Game.ReleaseDate)).ShouldBe(0);
        errors.Count(x => x.Identifier == nameof(Game.Game.AgeRating)).ShouldBe(3);
        errors.Count(x => x.Identifier == nameof(DeveloperInfo.Developer)).ShouldBe(1);
        errors.Count(x => x.Identifier == nameof(DiskSize)).ShouldBe(2);
        errors.Count(x => x.Identifier == nameof(Price)).ShouldBe(1);
        errors.Count(x => x.Identifier == nameof(GameDetails.Platform)).ShouldBe(1);
        errors.Count(x => x.Identifier == nameof(GameDetails.GameMode)).ShouldBe(2);
        errors.Count(x => x.Identifier == nameof(GameDetails.DistributionFormat)).ShouldBe(2);
        errors.Count(x => x.Identifier == nameof(SystemRequirements.Minimum)).ShouldBe(1);
        errors.Count(x => x.Identifier == nameof(Rating)).ShouldBe(0);
        errors.Count(x => x.Identifier == nameof(Game.Game.GameStatus)).ShouldBe(0);
    }

    [Fact]
    public async Task Create_User_Should_Return_Invalid_When_Required_Value_Objects_Are_Empty()
    {
        // Arrange
        var userEfRepository = Substitute.For<IUserEfRepository>();
        userEfRepository.EmailExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(false)); // or true, depending on your test

        var userResult = await User.User.CreateAsync(
            firstName: _faker.Name.FirstName(),
            lastName: _faker.Name.LastName(),
            email: string.Empty,
            password: string.Empty,
            role: string.Empty,
            userEfRepository
        );

        // Assert
        var errors = userResult.ValidationErrors;
        errors.ShouldNotBeNull()
            .ShouldNotBeEmpty();
        errors.ShouldBeOfType<List<ValidationError>>();
        errors.Count().ShouldBe(10);

        userResult.Status.ShouldBe(ResultStatus.Invalid);
        errors.Count(x => x.Identifier == nameof(User.User.FirstName)).ShouldBe(0);
        errors.Count(x => x.Identifier == nameof(User.User.LastName)).ShouldBe(0);
        errors.Count(x => x.Identifier == nameof(Email)).ShouldBe(2);
        errors.Count(x => x.Identifier == nameof(Password)).ShouldBe(6);
        errors.Count(x => x.Identifier == nameof(Role)).ShouldBe(2);
    }
}