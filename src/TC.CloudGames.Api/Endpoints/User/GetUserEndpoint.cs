using System.Net;
using Ardalis.Result;
using FastEndpoints;
using Microsoft.Extensions.Caching.Distributed;
using TC.CloudGames.Application.Middleware;
using TC.CloudGames.Application.Users.GetUser;

namespace TC.CloudGames.Api.Endpoints.User;

public sealed class GetUserEndpoint : Endpoint<GetUserQuery, UserResponse>
{
    private readonly IDistributedCache _cache;

    public GetUserEndpoint(IDistributedCache cache)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public override void Configure()
    {
        Get("user/{Id}");
        Roles("Admin");
        PostProcessor<CommandPostProcessor<GetUserQuery, UserResponse>>();

        Description(x => x.Produces<UserResponse>()
            .ProducesProblemDetails()
            .Produces((int)HttpStatusCode.NotFound)
            .Produces((int)HttpStatusCode.Forbidden)
            .Produces((int)HttpStatusCode.Unauthorized));

        Summary(s =>
        {
            s.Summary = "Retrieve user details by their unique identifier.";
            s.Description =
                "This endpoint retrieves detailed information about a user by their unique Id. Access is restricted to users with the appropriate role.";
            s.ExampleRequest = new GetUserQuery(Guid.NewGuid());
            s.ResponseExamples[200] = new UserResponse
            {
                Email = "John.smith@gmail.com", Id = Guid.NewGuid(), FirstName = "John", LastName = "Smith",
                Role = "Admin"
            };
            s.Responses[200] = "Returned when user information is successfully retrieved.";
            s.Responses[400] = "Returned when the request is invalid.";
            s.Responses[404] = "Returned when no user is found with the provided Id.";
            s.Responses[403] = "Returned when the caller lacks the required role to access this endpoint.";
            s.Responses[401] = "Returned when the request is made without a valid user token.";
        });
    }

    public override async Task HandleAsync(GetUserQuery req, CancellationToken ct)
    {
        // const string cacheKeyPrefix = "User-";
        // var response = await _cache.GetAsync(
        //     $"{cacheKeyPrefix}{req.Id}",
        //     async token => await req.ExecuteAsync(token),
        //     CacheOptions.DefaultExpiration,
        //     ct);

        var response = await req.ExecuteAsync(ct: ct).ConfigureAwait(false);

        if (response.IsSuccess)
        {
            await SendAsync(response.Value, cancellation: ct).ConfigureAwait(false);
            return;
        }

        if (response.IsNotFound())
        {
            await SendErrorsAsync((int)HttpStatusCode.NotFound, ct).ConfigureAwait(false);
            return;
        }

        await SendErrorsAsync(cancellation: ct).ConfigureAwait(false);
    }
}