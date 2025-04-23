using Ardalis.Result;
using Dapper;
using TC.CloudGames.Application.Abstractions.Data;
using TC.CloudGames.Application.Abstractions.Messaging;

namespace TC.CloudGames.Application.Users.GetUser
{
    internal sealed class GetUserQueryHandler : IQueryHandler<GetUserQuery, UserResponse>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public GetUserQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task<Result<UserResponse>> ExecuteAsync(GetUserQuery command, CancellationToken ct)
        {
            using var connection = await _sqlConnectionFactory.CreateConnectionAsync(ct).ConfigureAwait(false);

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

            var user = await connection.QuerySingleOrDefaultAsync<UserResponse>(sql, new { command.Id }).ConfigureAwait(false);
            if (user is null)
            {
                return Result<UserResponse>.NotFound($"User with id '{command.Id}' not found.");
            }

            return Result<UserResponse>.Success(user);
        }
    }
}
