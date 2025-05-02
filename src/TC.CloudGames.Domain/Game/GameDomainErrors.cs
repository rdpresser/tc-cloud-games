using TC.CloudGames.Domain.Abstractions;

namespace TC.CloudGames.Domain.Game;

public static class GameDomainErrors
{
    public static readonly DomainError NotFound = new(
        "Game.NotFound",
        "The game with the specified identifier was not found",
        "Game.NotFound");

    public static readonly DomainError CreateGame = new(
        "Game.CreateGame",
        "An error occurred while creating the game.",
        "Game.CreateGame");
}