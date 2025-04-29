using Ardalis.Result;
using System.Collections.Immutable;

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

            return Result<Role>.Error("Invalid role.");
        }

        public override string ToString() => Value;
    }
}
