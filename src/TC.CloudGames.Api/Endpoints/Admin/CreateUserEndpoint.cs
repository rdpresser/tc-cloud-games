using Ardalis.Result;
using FastEndpoints;
using FluentValidation;
using FluentValidation.Results;

namespace TC.CloudGames.Api.Endpoints.Admin
{
    public sealed class CreateUserEndpoint : Endpoint<RegisterRequest, Result<RegisterResponse>>
    {
        public override void Configure()
        {
            Post("identity/register");
            AllowAnonymous();
            DontAutoSendResponse();
            PostProcessor<ResponseSender>();   //register post processor
            Description(
                x => x.Produces<RegisterResponse>(200) //override swagger response type for 200 ok
                      .Produces<ErrorResponse>(400));
        }

        public override async Task<Result<RegisterResponse>> ExecuteAsync(RegisterRequest req, CancellationToken ct)
        {
            /*
             * Create any business logic here
             */

            await Task.CompletedTask; //simulate async work

            return Result<RegisterResponse>.Success(new(Guid.NewGuid(), req.Name, req.Email, req.Role));


            //return Result<RegisterResponse>.Invalid(
            //    new List<ValidationError>
            //    {
            //        new()
            //        {
            //            Identifier = nameof(RegisterRequest.Email),
            //            ErrorMessage = "I am unhappy!"
            //        }
            //    });
        }
    }

    public sealed class ResponseSender : IPostProcessor<RegisterRequest, Result<RegisterResponse>>
    {
        public async Task PostProcessAsync(IPostProcessorContext<RegisterRequest, Result<RegisterResponse>> ctx, CancellationToken ct)
        {
            if (!ctx.HttpContext.ResponseStarted())
            {
                var result = ctx.Response!;

                switch (result.Status)
                {
                    case ResultStatus.Ok:
                        await ctx.HttpContext.Response.SendAsync(result.GetValue());

                        break;

                    case ResultStatus.Invalid:
                        var failures = result.ValidationErrors.Select(e => new ValidationFailure(e.Identifier, e.ErrorMessage)).ToList();
                        await ctx.HttpContext.Response.SendErrorsAsync(failures);

                        break;
                }
            }
        }
    }


    public sealed class CreateUserValidator : Validator<RegisterRequest>
    {
        public CreateUserValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Invalid email format.");

            RuleFor(x => x.Role)
                .NotEmpty()
                .WithMessage("Role is required.")
                .Must(role => role == "Admin" || role == "User")
                .WithMessage("Role must be either 'Admin' or 'User'.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required.")
                .MinimumLength(8)
                .WithMessage("Password must be at least 8 characters long.")
                .Matches(@"[A-Z]")
                .WithMessage("Password must contain at least one uppercase letter.")
                .Matches(@"[a-z]")
                .WithMessage("Password must contain at least one lowercase letter.")
                .Matches(@"\d")
                .WithMessage("Password must contain at least one number.")
                .Matches(@"[\W_]")
                .WithMessage("Password must contain at least one special character.");
        }
    }

    public record RegisterRequest(string Name, string Email, string Password, string Role);

    public record RegisterResponse(Guid Id, string Name, string Email, string Role);
}
