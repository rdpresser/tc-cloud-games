using FastEndpoints;
using System.Net;
using TC.CloudGames.Api.Abstractions;
using TC.CloudGames.Application.Abstractions;
using TC.CloudGames.Application.Middleware;
using TC.CloudGames.Application.Users.GetUser;
using TC.CloudGames.Infra.CrossCutting.Commons.Authentication;
using ZiggyCreatures.Caching.Fusion;

namespace TC.CloudGames.Api.Endpoints.User;

public sealed class GetUserByEmailEndpoint : ApiEndpoint<GetUserByEmailQuery, UserByEmailResponse>
{
    public GetUserByEmailEndpoint(IFusionCache cache, IUserContext userContext)
        : base(cache, userContext)
    {

    }

    public override void Configure()
    {
        Get("user/by-email/{Email}");
        Roles(AppConstants.UserRole, AppConstants.AdminRole);
        PostProcessor<CommandPostProcessor<GetUserByEmailQuery, UserByEmailResponse>>();

        Description(x => x.Produces<UserByEmailResponse>()
            .ProducesProblemDetails()
            .Produces((int)HttpStatusCode.NotFound)
            .Produces((int)HttpStatusCode.Forbidden)
            .Produces((int)HttpStatusCode.Unauthorized));

        Summary(s =>
        {
            s.Summary = "Retrieve user details by their email.";
            s.Description =
                "This endpoint retrieves detailed information about a user by their Email. Access is restricted to users with the appropriate role.";
            s.ExampleRequest = new GetUserByEmailQuery("John.smith@gmail.com");
            s.ResponseExamples[200] = new UserByEmailResponse
            {
                Email = "John.smith@gmail.com",
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Smith",
                Role = "Admin"
            };
            s.Responses[200] = "Returned when user information is successfully retrieved.";
            s.Responses[400] = "Returned when the request is invalid.";
            s.Responses[404] = "Returned when no user is found with the provided Id.";
            s.Responses[403] = "Returned when the caller lacks the required role to access this endpoint.";
            s.Responses[401] = "Returned when the request is made without a valid user token.";
        });
    }

    public override async Task HandleAsync(GetUserByEmailQuery req, CancellationToken ct)
    {
        // Cache keys for user data and validation failures
        var userCacheKey = $"User-{req.Email}";
        var validationFailuresCacheKey = $"ValidationFailures-{userCacheKey}";

        // Use the helper to handle caching and validation
        var response = await GetOrSetWithValidationAsync
            (
                userCacheKey,
                validationFailuresCacheKey,
                req.ExecuteAsync,
                ct
            );

        // Use the MatchAsync method from the base class
        await MatchResultAsync(response, ct);
    }
}