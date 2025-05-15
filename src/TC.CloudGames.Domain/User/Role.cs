using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using System.Collections.Immutable;
using TC.CloudGames.Domain.Abstractions;
using TC.CloudGames.Infra.CrossCutting.Commons.Extensions;

namespace TC.CloudGames.Domain.User
{
    public sealed record Role
    {
        public string Value { get; }

        public static readonly IImmutableSet<string> ValidRoles =
            ImmutableHashSet.Create("Admin", "User");

        private Role(string value) => Value = value;

        public static Result<Role> Create(string value)
        {
            var role = new Role(value);
            var validator = new RoleValidator().ValidationResult(role);

            if (!validator.IsValid)
            {
                return Result.Invalid(validator.AsErrors());
            }

            return role;
        }

        public override string ToString() => Value;
    }

    public class RoleValidator : BaseValidator<Role>
    {
        public RoleValidator()
        {
            ValidateRole();
        }

        protected void ValidateRole()
        {
            RuleFor(x => x.Value)
                .NotEmpty()
                    .WithMessage("Role is required.")
                    .WithErrorCode($"{nameof(Role)}.Required")
                    .OverridePropertyName(nameof(Role))
                .Must(role => Role.ValidRoles.Contains(role))
                    .WithMessage($"Invalid role specified. Valid roles are: {Role.ValidRoles.JoinWithQuotes()}.")
                    .WithErrorCode($"{nameof(Role)}.InvalidRole")
                    .OverridePropertyName(nameof(Role));
        }
    }
}
