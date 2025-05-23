using Bogus;
using TC.CloudGames.Application.Users.GetUserList;
using TC.CloudGames.Infra.CrossCutting.Commons.Authentication;
using ZiggyCreatures.Caching.Fusion;

namespace TC.CloudGames.Api.Endpoints.User
{
    public sealed class GetUserListEndpoint : ApiEndpoint<GetUserListQuery, IReadOnlyList<UserListResponse>>
    {
        private static readonly string[] items = [AppConstants.AdminRole, AppConstants.UserRole];

        public GetUserListEndpoint(IFusionCache cache, IUserContext userContext)
            : base(cache, userContext)
        {
        }

        public override void Configure()
        {
            Get("user/list");
            Roles(AppConstants.AdminRole);
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
                    cacheKey,
                    validationFailuresCacheKey,
                    req.ExecuteAsync,
                    ct
                ).ConfigureAwait(false);

            // Use the MatchResultAsync method from the base class
            await MatchResultAsync(response, ct).ConfigureAwait(false);
        }
    }
}
