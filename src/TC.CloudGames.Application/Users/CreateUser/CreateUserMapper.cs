using TC.CloudGames.Domain.User;
using TC.CloudGames.Infra.CrossCutting.Commons.Authentication;
using TC.CloudGames.Infra.CrossCutting.Commons.Clock;

namespace TC.CloudGames.Application.Users.CreateUser;

public static class CreateUserMapper
{
    public static User ToEntity(CreateUserCommand r, IDateTimeProvider dateTimeProvider, IPasswordHasher passwordHasher)
    {
        return User.Create(
            firstName: r.FirstName,
            lastName: r.LastName,
            email: Email.Create(r.Email),
            password: Password.Create(r.Password),
            role: Role.Create(r.Role),
            createdOnUtc: dateTimeProvider.UtcNow
        );
    }

    public static CreateUserResponse FromEntity(User e)
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