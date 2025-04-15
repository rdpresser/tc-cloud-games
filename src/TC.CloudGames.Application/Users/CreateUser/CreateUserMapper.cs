using FastEndpoints;
using TC.CloudGames.Domain.User;

namespace TC.CloudGames.Application.Users.CreateUser
{
    public class CreateUserMapper : Mapper<CreateUserRequest, CreateUserResponse, User>
    {
        public override User ToEntity(CreateUserRequest r)
        {
            return User.Create(
                id: Guid.NewGuid(),
                firstName: new FirstName(r.FirstName),
                lastName: new LastName(r.LastName),
                email: new Email(r.Email),
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
