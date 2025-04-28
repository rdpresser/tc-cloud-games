using Ardalis.Result;
using Dapper;
using TC.CloudGames.Application.Abstractions.Data;
using TC.CloudGames.Application.Abstractions.Messaging;

namespace TC.CloudGames.Application.Games.GetGame
{
    internal sealed class GetGameQueryHandler : IQueryHandler<GetGameQuery, GameResponse>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public GetGameQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task<Result<GameResponse>> ExecuteAsync(GetGameQuery command, CancellationToken ct)
        {
            using var connection = await _sqlConnectionFactory.CreateConnectionAsync(ct).ConfigureAwait(false);

            const string sql = """
                    SELECT 
                        id AS Id, 
                        "name" AS Name, 
                        release_date AS ReleaseDate, 
                        age_rating AS AgeRating, 
                        description AS Description, 
                        disk_size_in_gb AS DiskSize,
                        price_amount AS Price,
                        rating AS Rating, 
                        official_link AS OfficialLink, 
                        game_status AS GameStatus,
                        developer_info AS Developer, 
                        developer_info_publisher AS Publisher,
                        playtime_hours AS Hours, 
                        playtime_player_count AS PlayerCount,
                        game_details_genre AS Genre, 
                        game_details_platform AS PlatformString, 
                        game_details_tags AS Tags, 
                        game_details_game_mode AS GameMode, 
                        game_details_distribution_format AS DistributionFormat, 
                        game_details_available_languages AS AvailableLanguages, 
                        game_details_supports_dlcs AS SupportsDlcs,
                        system_requirements_minimum AS Minimum, 
                        system_requirements_recommended AS Recommended
                    FROM public.games
                    WHERE id = @Id;
                    """;

            var game = await connection.QueryAsync<GameResponse, DeveloperInfo, Playtime, GameDetails, SystemRequirements, GameResponse>(
                sql,
                (game, developerInfo, playtime, gameDetails, systemRequirements) =>
                {
                    game.DeveloperInfo = developerInfo;
                    game.Playtime = playtime;
                    game.GameDetails = gameDetails;
                    game.SystemRequirements = systemRequirements;

                    return game;
                },
                new { command.Id },
                splitOn: "Developer,Hours,Genre,Minimum"
            ).ConfigureAwait(false);

            var result = game.FirstOrDefault();
            if (result is null)
            {
                return Result<GameResponse>.NotFound($"Game with id '{command.Id}' not found.");
            }

            return Result<GameResponse>.Success(result);
        }
    }
}
