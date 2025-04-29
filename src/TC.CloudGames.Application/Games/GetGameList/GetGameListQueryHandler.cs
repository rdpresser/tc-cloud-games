using Ardalis.Result;
using Dapper;
using TC.CloudGames.Application.Abstractions.Data;
using TC.CloudGames.Application.Abstractions.Messaging;
using TC.CloudGames.Application.Games.GetGame;

namespace TC.CloudGames.Application.Games.GetGameList
{
    internal sealed class GetGameListQueryHandler : IQueryHandler<GetGameListQuery, IReadOnlyList<GameListResponse>>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public GetGameListQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        private static readonly IReadOnlyDictionary<string, string> FieldMappings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "id" },
            { "Name", "name" },
            { "ReleaseDate", "release_date" },
            { "AgeRating", "age_rating" },
            { "Description", "description" },
            { "Developer", "developer_info" },
            { "Genre", "game_details_genre" },
            { "Platform", "game_details_platform" },
            { "Tags", "game_details_tags" },
            { "GameMode", "game_details_game_mode" },
            { "DistributionFormat", "game_details_distribution_format" },
            { "AvailableLanguages", "game_details_available_languages" },
            { "SupportsDlcs", "game_details_supports_dlcs" },
            { "Minimum", "system_requirements_minimum" },
            { "Recommended", "system_requirements_recommended" }
        };

        public async Task<Result<IReadOnlyList<GameListResponse>>> ExecuteAsync(GetGameListQuery query, CancellationToken ct)
        {
            using var connection = await _sqlConnectionFactory.CreateConnectionAsync(ct).ConfigureAwait(false);

            var orderByField = FieldMappings.TryGetValue(query.SortBy, out var mappedField) ? mappedField : "id";
            var orderByClause = $"{orderByField} {query.SortDirection.ToUpper()}";
            if (string.IsNullOrWhiteSpace(query.SortBy) || string.IsNullOrWhiteSpace(query.SortDirection))
            {
                orderByClause = "id ASC"; // Default ordering
            }

            var sql = $"""
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
                WHERE  
                    (@Filter IS NULL OR id::text ILIKE '%' || @Filter || '%' OR  
                     name ILIKE '%' || @Filter || '%' OR  
                     description ILIKE '%' || @Filter || '%' OR  
                     developer_info ILIKE '%' || @Filter || '%' OR  
                     game_details_genre ILIKE '%' || @Filter || '%' OR  
                     game_details_platform ILIKE '%' || @Filter || '%')  
                ORDER BY {orderByClause}  
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;

            var parameters = new
            {
                Offset = (query.PageNumber - 1) * query.PageSize,
                query.PageSize,
                query.SortBy,
                SortDirection = string.Equals(query.SortDirection, "DESC", StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC",
                query.Filter
            };

            var games = await connection
                .QueryAsync<GameListResponse, DeveloperInfo, Playtime, GameDetails, SystemRequirements, GameListResponse>(
                sql,
                (game, developerInfo, playtime, gameDetails, systemRequirements) =>
                {
                    game.DeveloperInfo = developerInfo;
                    game.Playtime = playtime;
                    game.GameDetails = gameDetails;
                    game.SystemRequirements = systemRequirements;

                    return game;
                },
                parameters,
                splitOn: "Developer,Hours,Genre,Minimum"
            ).ConfigureAwait(false);


            if (games is null || !games.Any())
            {
                return Result<IReadOnlyList<GameListResponse>>.Success([]);
            }

            return Result.Success<IReadOnlyList<GameListResponse>>([.. games]);
        }
    }
}
