using Bogus;
using FastEndpoints;
using System.Net;
using TC.CloudGames.Application.Users.GetUserList;

namespace TC.CloudGames.Api.Endpoints.User
{
    public sealed class GetUserListEndpoint : Endpoint<GetUserListQuery, IReadOnlyList<UserListResponse>>
    {
        private static readonly string[] items = ["Admin", "User"];

        public override void Configure()
        {
            Get("user/list");
            Roles("Admin");
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
            var response = await req.ExecuteAsync(ct: ct).ConfigureAwait(false);

            if (response.IsSuccess)
            {
                await SendAsync(response.Value, cancellation: ct).ConfigureAwait(false);
                return;
            }

            await SendErrorsAsync(cancellation: ct).ConfigureAwait(false);
        }
    }
}
