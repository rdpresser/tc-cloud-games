using FastEndpoints;
using TC.CloudGames.Domain.User;

namespace TC.CloudGames.Application.Users.CreateUser
{
    public sealed class CreateUserMapper : Mapper<CreateUserCommand, CreateUserResponse, User>
    {
        public override User ToEntity(CreateUserCommand r)
        {
            return User.Create(
                firstName: r.FirstName,
                lastName: r.LastName,
                email: Email.Create(r.Email),
                password: Password.Create(r.Password),
                role: Role.Create(r.Role)
            );
        }

        public override CreateUserResponse FromEntity(User e)
        {
            return new CreateUserResponse
            (
                e.Id,
                e.FirstName,
                e.LastName,
                e.Email.Value,
                e.Role.Value
            );
        }
    }
}
