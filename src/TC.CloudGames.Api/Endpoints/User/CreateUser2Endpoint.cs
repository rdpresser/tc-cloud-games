//using FastEndpoints;
//using Microsoft.AspNetCore.Http.HttpResults;

//namespace TC.CloudGames.Api.Endpoints.Admin
//{
//    public sealed class CreateUser2Endpoint : Endpoint<RegisterRequest, Results<Ok<RegisterResponse>, NotFound, ErrorResponse>>
//    {
//        public override void Configure()
//        {
//            Post("identity/register2");
//            AllowAnonymous();
//            Description(
//                x => x.Produces<RegisterResponse>(200) //override swagger response type for 200 ok
//                      .Produces<ProblemDetails>(400));
//        }


//        //public override async Task HandleAsync(Register3Request req, CancellationToken ct)
//        //{
//        //    AddError(r => r.Email, "this email is already in use!");
//        //    AddError(r => r.Name, "you are not eligible for insurance!");

//        //    //ThrowIfAnyErrors(); // If there are errors, execution shouldn't go beyond this point

//        //    //ThrowError(r => r.Email, "creating a user did not go so well!"); // Error response sent here

//        //    if (ValidationFailed)
//        //    {
//        //        await SendErrorsAsync(cancellation: ct);
//        //        return;
//        //    }

//        //    await SendAsync(new(Guid.NewGuid().ToString(), req.Name, req.Email, req.Role), cancellation: ct);
//        //}

//        public override async Task<Results<Ok<RegisterResponse>, NotFound, ErrorResponse>> ExecuteAsync(RegisterRequest req, CancellationToken ct)
//        {
//            await Task.CompletedTask; //simulate async work

//            if (req.Name == "notfound") //condition for a not found response
//            {
//                return TypedResults.NotFound();
//            }

//            if (req.Name == "1") //condition for a problem details response
//            {
//                AddError(r => r.Password, "name is invalid");
//                AddError(r => r.Email, "email is invalid");
//                AddError(r => r.Password, "name is invalid2");
//                return new ErrorResponse(ValidationFailures);
//            }

//            // 200 ok response with a DTO
//            return TypedResults.Ok(new RegisterResponse(Guid.NewGuid(), req.Name, req.Email, req.Role));
//        }
//    }
//}
