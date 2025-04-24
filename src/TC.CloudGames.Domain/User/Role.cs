using Ardalis.Result;

namespace TC.CloudGames.Domain.User
{
    public sealed record Role
    {
        public string Value { get; init; }

        public static readonly string[] AllowedRoles = { "Admin", "User" };

        private Role(string value) => Value = value;

        public static Result<Role> Create(string value)
        {
            if (IsValidRole(value))
            {
                return Result.Success(new Role(value));
            }

            return Result<Role>.Error("Invalid role.");
        }

        public static bool IsValidRole(string value)
        {
            return Array.Exists(AllowedRoles, role => role == value);
        }

        public override string ToString() => Value;
    }
}
