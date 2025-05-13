using Ardalis.Result;
using Bogus;
using FluentAssertions;
using TC.CloudGames.Domain.Game;

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
        
        _genres = new List<string> { "Ação", "Aventura", "RPG", "Estratégia", "Simulação", "Corrida", "Esporte", "Puzzle", 
            "Luta", "Plataforma", "FPS", "TPS", "Survival", "Horror", "Stealth", "Mundo Aberto", "MMORPG", "Roguelike", 
            "Visual Novel", "Beat 'em up", "Battle Royale", "Musical", "Party Game", "Metroidvania", "Idle / Incremental",
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
    public void Create_Should_Return_Invalid_When_Name_Is_Empty()
    {
        // Arrange
        var result = Game.Game.Create(
            name: "",
            releaseDate: DateOnly.FromDateTime(_faker.Date.Past(2)),
            ageRating: AgeRating.Create(_faker.PickRandom("E", "E10+", "T", "M", "A", "RP")),
            description: _faker.Lorem.Paragraph(),
            developerInfo: new DeveloperInfo(_faker.Company.CompanyName(), _faker.Company.CompanyName()),
            diskSize: new DiskSize(_faker.Random.Int(1, 150)),
            price: new Price(decimal.Parse(_faker.Commerce.Price(200.0m, 500.0m))),
            playtime: new Playtime(_faker.Random.Int(1, 1000), _faker.Random.Int(1, 1000000)),
            gameDetails: GameDetails.Create(
                _faker.PickRandom(_genres), 
                _faker.PickRandom(_platforms, 2).ToArray(), 
                _faker.PickRandom(_gameTags), 
                _faker.PickRandom(_gameModes), 
                _faker.PickRandom(_distributionFormats), 
                _faker.PickRandom(_languages), 
                _faker.Random.Bool()), 
            systemRequirements: new SystemRequirements(_faker.Lorem.Paragraph(), _faker.Lorem.Paragraph()),
            rating: Rating.Create(Math.Round(_faker.Random.Decimal(1, 10), 2)), 
            officialLink: _faker.Internet.Url(),
            gameStatus: _faker.PickRandom(_gameStatus),
            createdOnUtc: DateTime.UtcNow
        );
        
        // Assert
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ValidationErrors.Should().ContainSingle(x => x.Identifier == "Name");
    }
}