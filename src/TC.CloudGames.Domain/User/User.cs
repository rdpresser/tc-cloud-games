using Ardalis.Result;
using TC.CloudGames.Domain.Abstractions;

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
        //EF Core
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


    public static Result<User> Create(string firstName, string lastName, Email email, Password password, Role role)
    {
        List<ValidationError> validation = [];

        if (string.IsNullOrWhiteSpace(firstName))
        {
            validation.Add(new()
            {
                Identifier = nameof(FirstName),
                ErrorMessage = "Firstname is required.",
                ErrorCode = $"{nameof(FirstName)}.Required"
            });
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            validation.Add(new()
            {
                Identifier = nameof(LastName),
                ErrorMessage = "Lastname is required.",
                ErrorCode = $"{nameof(LastName)}.Required"
            });
        }

        if (validation.Count != 0)
        {
            return Result<User>.Invalid(validation);
        }

        var user = new User(Guid.NewGuid(), firstName, lastName, email, password, role);

        /*
         * RaiseDomainEvent - Send onboarding email to the new user
         */

        return user;
    }
}