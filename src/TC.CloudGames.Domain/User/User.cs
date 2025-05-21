using TC.CloudGames.Domain.User.Abstractions;

namespace TC.CloudGames.Domain.User;

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

    public static async Task<Result<User>> CreateAsync(string firstName, string lastName, string email, string password, string role, IUserEfRepository userEfRepository)
    {
        var emailResult = await Email.CreateAsync(builder => builder.Value = email, userEfRepository).ConfigureAwait(false);
        var passwordResult = Password.Create(builder => builder.Value = password);
        var roleResult = Role.Create(builder => builder.Value = role);

        var valueObjectResults = new IResult[]
        {
            emailResult,
            passwordResult,
            roleResult
        };

        var errors = CollectValidationErrors(valueObjectResults);
        if (errors.Count != 0)
        {
            return Result.Invalid(errors);
        }

        var user = new User(Guid.NewGuid(), firstName, lastName, emailResult.Value, passwordResult.Value, roleResult.Value);
        var validator = await new CreateUserValidator().ValidationResultAsync(user).ConfigureAwait(false);

        if (!validator.IsValid)
            return Result.Invalid(validator.AsErrors());

        /*
         * RaiseDomainEvent - Send onboarding email to the new user
         */

        return user;
    }
}