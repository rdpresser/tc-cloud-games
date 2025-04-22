using Ardalis.Result;
using Dapper;
using TC.CloudGames.Application.Abstractions.Data;
using TC.CloudGames.Application.Abstractions.Messaging;

namespace TC.CloudGames.Application.Users.GetUserList
{
    internal sealed class GetUserListQueryHandler : IQueryHandler<GetUserListQuery, IReadOnlyList<UserListResponse>>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public GetUserListQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        private static readonly IReadOnlyDictionary<string, string> FieldMappings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "id" },
            { "FirstName", "first_name" },
            { "LastName", "last_name" },
            { "Email", "email" },
            { "Role", "role" }
        };

        public async Task<Result<IReadOnlyList<UserListResponse>>> ExecuteAsync(GetUserListQuery query, CancellationToken ct)
        {
            using var connection = await _sqlConnectionFactory.CreateConnectionAsync(ct);

            var orderByField = FieldMappings.TryGetValue(query.SortBy, out var mappedField) ? mappedField : "id";
            var orderByClause = $"{orderByField} {query.SortDirection.ToUpper()}";
            if (string.IsNullOrWhiteSpace(query.SortBy) || string.IsNullOrWhiteSpace(query.SortDirection))
            {
                orderByClause = "id ASC"; // Default ordering
            }

            var sql = $"""
                SELECT   
                    id AS Id,   
                    first_name AS FirstName,   
                    last_name AS LastName,   
                    email AS Email,   
                    role AS Role  
                FROM public.users  
                WHERE  
                    (@Filter IS NULL OR id::text ILIKE '%' || @Filter || '%' OR  
                     first_name ILIKE '%' || @Filter || '%' OR  
                     last_name ILIKE '%' || @Filter || '%' OR  
                     email ILIKE '%' || @Filter || '%' OR  
                     role ILIKE '%' || @Filter || '%')  
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

            var users = await connection
                .QueryAsync<UserListResponse>(sql, parameters);

            if (users is null || !users.Any())
            {
                return Result<IReadOnlyList<UserListResponse>>.Success([]);
            }

            return Result.Success<IReadOnlyList<UserListResponse>>([.. users]);
        }
    }
}
