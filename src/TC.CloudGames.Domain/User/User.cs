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

    private User(Guid id, string firstName, string lastName, Email email, Password password, Role role,
        DateTime createdOnUtc)
        : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Password = password;
        Role = role;
        CreatedOnUtc = createdOnUtc;
    }


    public static User Create(string firstName, string lastName, Email email, Password password, Role role,
        DateTime createdOnUtc)
    {
        var user = new User(Guid.NewGuid(), firstName, lastName, email, password, role, createdOnUtc);

        /*
         * RaiseDomainEvent - Send onboarding email to the new user
         */

        return user;
    }

    /// <summary>
    /// Method used only for Seed operation
    /// </summary>
    /// <param name="firstName"></param>
    /// <param name="lastName"></param>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <param name="role"></param>
    /// <param name="createdOnUtc"></param>
    /// <returns></returns>
    public static User CreateWithIdForDbSeed(Guid id, string firstName, string lastName, Email email, Password password, Role role,
        DateTime createdOnUtc)
    {
        return new User(id, firstName, lastName, email, password, role, createdOnUtc);
    }
}