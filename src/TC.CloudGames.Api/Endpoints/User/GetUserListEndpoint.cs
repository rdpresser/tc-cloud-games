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
            for (int i = 0; i < 4; i++)
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
                s.Summary = "Endpoint responsible for retrieving a user list with pagination filters";
                s.ExampleRequest = new GetUserListQuery(PageNumber: 1, PageSize: 10, SortBy: "id", SortDirection: "asc", Filter: "<any value>");
                s.ResponseExamples[200] = userList;
                s.Responses[200] = "Occurs when a user list are successfully retrieved using filters";
                s.Responses[400] = "Occurs when a badrequest happens";
                s.Responses[403] = "Occurs when a logged user doesn't have the appropriate role for this endpoint";
                s.Responses[401] = "Occurs when the endpoint is called without an user token";
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

            response.Errors.ToList().ForEach(e => AddError(e));
            await SendErrorsAsync(cancellation: ct).ConfigureAwait(false);
        }
    }
}
