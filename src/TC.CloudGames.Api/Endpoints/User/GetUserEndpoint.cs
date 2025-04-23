using Ardalis.Result;
using FastEndpoints;
using System.Net;
using TC.CloudGames.Application.Users.GetUser;

namespace TC.CloudGames.Api.Endpoints.User
{
    public sealed class GetUserEndpoint : Endpoint<GetUserQuery, UserResponse>
    {
        public override void Configure()
        {

            Get("user/{Id}");
            Roles("Admin");
            Description(
                x => x.Produces<UserResponse>(200)
                      .ProducesProblemDetails()
                      .Produces((int)HttpStatusCode.NotFound)
                      .Produces((int)HttpStatusCode.Forbidden)
                      .Produces((int)HttpStatusCode.Unauthorized));

            Summary(s =>
            {
                s.Summary = "Endpoint responsible for retrieving user data by Id";
                s.ExampleRequest = new GetUserQuery(Guid.NewGuid());
                s.ResponseExamples[200] = new UserResponse { Email = "jhon.smith@gmail.com", Id = Guid.NewGuid(), FirstName = "Jhon", LastName = "Smith", Role = "Admin" };
                s.Responses[200] = "Occurs when a user successfully retrieves user information by id";
                s.Responses[400] = "Occurs when a badrequest happens";
                s.Responses[404] = "Occurs when an user Id is not found";
                s.Responses[403] = "Occurs when a logged user doesn't have the appropriate role for this endpoint";
                s.Responses[401] = "Occurs when the endpoint is called without an user token";
            });
        }

        public override async Task HandleAsync(GetUserQuery req, CancellationToken ct)
        {
            var response = await req.ExecuteAsync(ct: ct).ConfigureAwait(false);

            if (response.IsSuccess)
            {
                await SendAsync(response.Value, cancellation: ct).ConfigureAwait(false);
                return;
            }

            response.Errors.ToList().ForEach(e => AddError(e));

            if (response.IsNotFound())
            {
                await SendErrorsAsync((int)HttpStatusCode.NotFound, cancellation: ct).ConfigureAwait(false);
                return;
            }

            await SendErrorsAsync(cancellation: ct).ConfigureAwait(false);
        }
    }
}
