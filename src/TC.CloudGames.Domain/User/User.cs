using TC.CloudGames.Domain.Abstractions;

namespace TC.CloudGames.Domain.User
{
    public sealed class User : Entity
    {
        public FirstName FirstName { get; private set; }
        public LastName LastName { get; private set; }
        public Email Email { get; private set; }
        public Password Password { get; private set; } //TODO: Verificar onde o password será gravado
        public Role Role { get; private set; }

        private User()
        {
            //EF Core
        }

        private User(Guid id, FirstName firstName, LastName lastName, Email email, Password password, Role role)
            : base(id)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Password = password;
            Role = role;
        }

        public static User Create(FirstName firstName, LastName lastName, Email email, Password password, Role role)
        {
            var user = new User(Guid.NewGuid(), firstName, lastName, email, password, role);

            /*
             * RaiseDomainEvent - Send onboarding email to the new user
             */

            return user;
        }
    }
}
