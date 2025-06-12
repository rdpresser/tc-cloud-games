using TC.CloudGames.Domain.Aggregates.User.Abstractions;
using TC.CloudGames.Domain.Aggregates.User.ValueObjects;
using TC.CloudGames.Domain.Entities;

namespace TC.CloudGames.Domain.Aggregates.User;

public sealed class User : Entity
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public Email Email { get; private set; }
    public Password Password { get; private set; }
    public Role Role { get; private set; }

    private User()
    {
    }

    private User(Guid id, string firstName, string lastName, Email email, Password password, Role role)
        : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Password = password;
        Role = role;
    }

    // Builder pattern using raw values
    public static async Task<Result<User>> CreateAsync(Action<UserBuilder> configure, IUserEfRepository userEfRepository)
    {
        var builder = new UserBuilder();
        configure(builder);
        return await builder.BuildAsync(userEfRepository).ConfigureAwait(false);
    }

    // Builder pattern using value objects
    public static Result<User> CreateFromValueObjects(Action<UserBuilderFromValueObjects> configure)
    {
        var builder = new UserBuilderFromValueObjects();
        configure(builder);
        return builder.Build();
    }

    // Builder pattern using Result-wrapped value objects
    public static Result<User> CreateFromResult(Action<UserBuilderFromResultValueObjects> configure)
    {
        var builder = new UserBuilderFromResultValueObjects();
        configure(builder);
        return builder.Build();
    }

    public class UserBuilder
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

        public async Task<Result<User>> BuildAsync(IUserEfRepository userEfRepository)
        {
            var emailResult = await ValueObjects.Email.CreateAsync(builder => builder.Value = Email, userEfRepository).ConfigureAwait(false);
            var passwordResult = ValueObjects.Password.Create(builder => builder.Value = Password);
            var roleResult = ValueObjects.Role.Create(builder => builder.Value = Role);

            return BuildUser(
                FirstName,
                LastName,
                emailResult,
                passwordResult,
                roleResult
            );
        }
    }

    public class UserBuilderFromValueObjects
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public Email Email { get; set; }
        public Password Password { get; set; }
        public Role Role { get; set; }

        public Result<User> Build()
        {
            return BuildUser(
                FirstName,
                LastName,
                EnsureResult(Email, nameof(ValueObjects.Email)),
                EnsureResult(Password, nameof(ValueObjects.Password)),
                EnsureResult(Role, nameof(ValueObjects.Role))
            );
        }
    }

    public class UserBuilderFromResultValueObjects
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public Result<Email> Email { get; set; }
        public Result<Password> Password { get; set; }
        public Result<Role> Role { get; set; }

        public Result<User> Build()
        {
            return BuildUser(
                FirstName,
                LastName,
                Email,
                Password,
                Role
            );
        }
    }

    // Shared construction and validation logic
    private static Result<User> BuildUser(
        string firstName,
        string lastName,
        Result<Email> emailResult,
        Result<Password> passwordResult,
        Result<Role> roleResult)
    {
        var valueObjectResults = new IResult[]
        {
            EnsureResult(emailResult, nameof(ValueObjects.Email)),
            EnsureResult(passwordResult, nameof(ValueObjects.Password)),
            EnsureResult(roleResult , nameof(ValueObjects.Role))
        };

        var errors = CollectValidationErrors(valueObjectResults).ToList();

        var user = new User(Guid.NewGuid(), firstName, lastName, emailResult.Value, passwordResult.Value, roleResult.Value);

        var validator = new CreateUserValidator().ValidationResult(user);
        if (!validator.IsValid)
        {
            errors.AddRange(validator.AsErrors());
        }

        if (errors.Count != 0)
        {
            return Result.Invalid(errors);
        }

        /*
         * RaiseDomainEvent - Send onboarding email to the new user
         */

        return user;
    }
}