using FastEndpoints;
using TC.CloudGames.Domain.User;

namespace TC.CloudGames.Application.Users.CreateUser
{
    public sealed class CreateUserMapper : Mapper<CreateUserCommand, CreateUserResponse, User>
    {
        public override User ToEntity(CreateUserCommand r)
        {
            return User.Create(
                firstName: new FirstName(r.FirstName),
                lastName: new LastName(r.LastName),
                email: new Email(r.Email),
                password: new Password(r.Password),
                role: new Role(r.Role)
            );
        }

        public override CreateUserResponse FromEntity(User e)
        {
            return new CreateUserResponse
            (
                e.Id,
                e.FirstName.Value,
                e.LastName.Value,
                e.Email.Value,
                e.Role.Value
            );
        }
    }
}
