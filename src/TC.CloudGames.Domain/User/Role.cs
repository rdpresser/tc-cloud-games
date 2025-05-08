using Ardalis.Result;
using System.Collections.Immutable;
using TC.CloudGames.CrossCutting.Commons.Extensions;

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
            if (ValidRoles.Contains(value))
            {
                return Result<Role>.Success(new Role(value));
            }

            return Result<Role>.Invalid(new ValidationError
            {
                Identifier = nameof(Role),
                ErrorMessage = $"Invalid role specified. Valid roles are: {ValidRoles.JoinWithQuotes()}.",
                ErrorCode = $"{nameof(Role)}.Invalid"
            });
        }

        public override string ToString() => Value;
    }
}
