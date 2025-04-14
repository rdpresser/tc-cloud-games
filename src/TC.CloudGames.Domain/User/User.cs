using TC.CloudGames.Domain.Abstractions;

namespace TC.CloudGames.Domain.User
{
    public sealed class User : Entity
    {
        public FirstName FirstName { get; private set; }
        public LastName LastName { get; private set; }
        public Email Email { get; private set; }
        public Role Role { get; private set; }

        private User(Guid id, FirstName firstName, LastName lastName, Email email, Role role)
            : base(id)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Role = role;
        }

        public static User Create(Guid id, FirstName firstName, LastName lastName, Email email, Role role)
        {
            var user = new User(id, firstName, lastName, email, role);

            /*
             * RaiseDomainEvent - Send onboarding email to the new user
             */

            return user;
        }
    }
}
