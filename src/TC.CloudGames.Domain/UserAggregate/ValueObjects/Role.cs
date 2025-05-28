using System.Collections.Immutable;
using TC.CloudGames.Infra.CrossCutting.Commons.Extensions;

namespace TC.CloudGames.Domain.UserAggregate.ValueObjects
{
    public sealed record Role
    {
        public string Value { get; }

        public static readonly IImmutableSet<string> ValidRoles =
            ImmutableHashSet.Create("Admin", "User");

        private Role(string value) => Value = value;

        public static Result<Role> Create(Action<RoleBuilder> configure)
        {
            var builder = new RoleBuilder();
            configure(builder);
            return builder.Build();
        }

        public class RoleBuilder
        {
            public string Value { get; set; } = string.Empty;

            public Result<Role> Build()
            {
                var role = new Role(Value);
                var validator = new RoleValidator().ValidationResult(role);

                if (!validator.IsValid)
                {
                    return Result.Invalid(validator.AsErrors());
                }

                return role;
            }
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
                    .OverridePropertyName(nameof(Role))
                .MaximumLength(20)
                    .WithMessage("Role cannot exceed 20 characters.")
                    .WithErrorCode($"{nameof(Role)}.MaximumLength")
                    .OverridePropertyName(nameof(Role));
        }
    }
}
