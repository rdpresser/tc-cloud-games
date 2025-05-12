using Bogus;
using FastEndpoints;
using System.Net;
using TC.CloudGames.Api.Abstractions;
using TC.CloudGames.Application.Middleware;
using TC.CloudGames.Application.Users.GetUserList;
using TC.CloudGames.Infra.CrossCutting.Commons.Authentication;
using ZiggyCreatures.Caching.Fusion;

namespace TC.CloudGames.Api.Endpoints.User
{
    public sealed class GetUserListEndpoint : ApiEndpoint<GetUserListQuery, IReadOnlyList<UserListResponse>>
    {
        private static readonly string[] items = ["Admin", "User"];
        private readonly IFusionCache _cache;
        private readonly IUserContext _userContext;

        public GetUserListEndpoint(IFusionCache cache, IUserContext userContext)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _userContext = userContext;
        }

        public override void Configure()
        {
            Get("user/list");
            Roles("Admin");
            PostProcessor<CommandPostProcessor<GetUserListQuery, IReadOnlyList<UserListResponse>>>();

            Description(
                x => x.Produces<UserListResponse>(200)
                      .ProducesProblemDetails()
                      .Produces((int)HttpStatusCode.Forbidden)
                      .Produces((int)HttpStatusCode.Unauthorized));

            var faker = new Faker();
            List<UserListResponse> userList = [];
            for (int i = 0; i < 5; i++)
            {
                userList.Add(new UserListResponse
                {
                    Id = Guid.NewGuid(),
                    FirstName = faker.Name.FirstName(),
                    LastName = faker.Name.LastName(),
                    Email = faker.Internet.Email(),
                    Role = faker.PickRandom(items)
                });
            }

            Summary(s =>
            {
                s.Summary = "Retrieves a paginated list of users based on the provided filters.";
                s.ExampleRequest = new GetUserListQuery(PageNumber: 1, PageSize: 10, SortBy: "id", SortDirection: "asc", Filter: "<any value/field>");
                s.ResponseExamples[200] = userList;
                s.Responses[200] = "Returned when the user list is successfully retrieved using the specified filters.";
                s.Responses[400] = "Returned when the request contains invalid parameters.";
                s.Responses[403] = "Returned when the logged-in user lacks the required role to access this endpoint.";
                s.Responses[401] = "Returned when the request is made without a valid user token.";
                s.Responses[404] = "Returned when no users are found matching the specified filters.";
            });
        }

        public override async Task HandleAsync(GetUserListQuery req, CancellationToken ct)
        {
            // Cache keys for user data and validation failures
            var cacheKey = $"UserList-{req.PageNumber}-{req.PageSize}-{req.SortBy}-{req.SortDirection}-{req.Filter}";
            var validationFailuresCacheKey = $"ValidationFailures-{cacheKey}";

            // Use the helper to handle caching and validation
            var response = await GetOrSetWithValidationAsync
                (
                    _cache,
                    cacheKey,
                    validationFailuresCacheKey,
                    req.ExecuteAsync,
                    ValidationFailures,
                    _userContext,
                    ct
                );

            // Use the MatchResultAsync method from the base class
            await MatchResultAsync(response, ct);
        }
    }
}
