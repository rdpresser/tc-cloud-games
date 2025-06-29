using Dapper;
using TC.CloudGames.Application.Abstractions.Data;
using TC.CloudGames.Application.Users.GetUserByEmail;
using TC.CloudGames.Application.Users.GetUserById;
using TC.CloudGames.Application.Users.GetUserList;
using TC.CloudGames.Infra.Data.Configurations.Connection;

namespace TC.CloudGames.Infra.Data.Repositories.PostgreSql;

public class UserPgRepository : PgRepository, IUserPgRepository
{
    private readonly IReadOnlyDictionary<string, string> _fieldMappings =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "id" },
            { "FirstName", "first_name" },
            { "LastName", "last_name" },
            { "Email", "email" },
            { "Role", "role" }
        };

    public UserPgRepository(IPgDbConnectionProvider connectionProvider)
        : base(connectionProvider)
    {

    }

    public async Task<UserByIdResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var connection = await ConnectionProvider
            .CreateConnectionAsync(cancellationToken).ConfigureAwait(false);

        const string sql = """
                           SELECT 
                               id AS Id, 
                               first_name AS FirstName, 
                               last_name AS LastName, 
                               email AS Email, 
                               role AS Role
                           FROM public.users
                           WHERE id = @Id;
                           """;

        return await connection.QuerySingleOrDefaultAsync<UserByIdResponse>(sql, new { id })
            .ConfigureAwait(false);
    }

    public async Task<UserByEmailResponse?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        await using var connection = await ConnectionProvider
            .CreateConnectionAsync(cancellationToken)
            .ConfigureAwait(false);

        const string sql = """
                           SELECT 
                               id AS Id, 
                               first_name AS FirstName, 
                               last_name AS LastName, 
                               email AS Email, 
                               role AS Role
                           FROM public.users
                           WHERE UPPER(email) = UPPER(@Email);
                           """;

        return await connection.QuerySingleOrDefaultAsync<UserByEmailResponse>(sql, new { email })
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<UserListResponse>> GetUserListAsync(GetUserListQuery query, CancellationToken cancellationToken = default)
    {
        await using var connection = await ConnectionProvider
            .CreateConnectionAsync(cancellationToken)
            .ConfigureAwait(false);

        var orderByField = _fieldMappings.GetValueOrDefault(query.SortBy, "id");
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
            SortDirection = string.Equals(query.SortDirection, "DESC", StringComparison.OrdinalIgnoreCase)
                ? "DESC"
                : "ASC",
            query.Filter
        };

        var users = await connection
            .QueryAsync<UserListResponse>(sql, parameters)
            .ConfigureAwait(false);

        return [.. users];
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        await using var connection = await ConnectionProvider
            .CreateConnectionAsync(cancellationToken)
            .ConfigureAwait(false);

        const string sql = """
                           SELECT 1
                           FROM public.users
                           WHERE UPPER(email) = UPPER(@Email)
                           """;

        var result = await connection.QueryFirstOrDefaultAsync<int?>(sql, new { Email = email })
            .ConfigureAwait(false);

        return result.HasValue;
    }
}