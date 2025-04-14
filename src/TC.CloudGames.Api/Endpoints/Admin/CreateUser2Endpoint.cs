using Ardalis.Result;
using FastEndpoints;
using FluentValidation;

namespace TC.CloudGames.Api.Endpoints.Admin
{
    public class CreateUser2Endpoint : Endpoint<Register2Request, Register2Response>
    {
        public override void Configure()
        {
            Post("identity/register2");
            AllowAnonymous();
            Description(
                x => x.Produces<Register2Response>(200) //override swagger response type for 200 ok
                      .Produces<ErrorResponse>(400));
        }

        public override async Task HandleAsync(Register2Request r, CancellationToken c)
        {
            //var result = Result<string>.Invalid(new List<ValidationError>
            //{
            //    new ValidationError
            //    {
            //        Identifier = nameof(Register2Request.Email),
            //        ErrorMessage = "I am unhappy!"
            //    }
            //});

            var result = Result<Register2Response>.Success(new(Guid.NewGuid(), r.Name, r.Email, r.Role));

            await this.SendResponse(result, r => r.Value);
        }

        //public override async Task<Register2Response> ExecuteAsync(Register2Request req, CancellationToken ct)
        //{
        //    /*
        //     * Create any business logic here
        //     */

        //    await Task.CompletedTask; //simulate async work


        //}
    }

    public sealed class CreateUser2Validator : Validator<Register2Request>
    {
        public CreateUser2Validator()
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

    public record Register2Request(string Name, string Email, string Password, string Role);

    public record Register2Response(Guid Id, string Name, string Email, string Role);

    static class ArdalisResultsExtensions
    {
        public static async Task SendResponse<TResult, TResponse>(this IEndpoint ep, TResult result, Func<TResult, TResponse> mapper) where TResult : Ardalis.Result.IResult
        {
            switch (result.Status)
            {
                case ResultStatus.Ok:
                    await ep.HttpContext.Response.SendAsync(mapper(result));
                    break;

                case ResultStatus.Invalid:
                    result.ValidationErrors.ToList().ForEach(e =>
                        ep.ValidationFailures.Add(new(e.Identifier, e.ErrorMessage)));

                    await ep.HttpContext.Response.SendErrorsAsync(ep.ValidationFailures);
                    break;
            }
        }
    }
}
