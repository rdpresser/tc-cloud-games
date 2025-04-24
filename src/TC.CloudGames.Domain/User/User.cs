using TC.CloudGames.Domain.Abstractions;

namespace TC.CloudGames.Domain.User
{
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

        public static User Create(string firstName, string lastName, Email email, Password password, Role role)
        {
            var user = new User(Guid.NewGuid(), firstName, lastName, email, password, role);

            /*
             * RaiseDomainEvent - Send onboarding email to the new user
             */

            return user;
        }
    }
}
