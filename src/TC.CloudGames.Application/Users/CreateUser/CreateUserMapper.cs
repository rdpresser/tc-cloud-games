using TC.CloudGames.Domain.UserAggregate;
using TC.CloudGames.Domain.UserAggregate.Abstractions;

namespace TC.CloudGames.Application.Users.CreateUser;

public static class CreateUserMapper
{
    public static async Task<Result<User>> ToEntityAsync(CreateUserCommand r, IUserEfRepository userEfRepository)
    {
        return await User.CreateAsync(builder =>
        {
            builder.FirstName = r.FirstName;
            builder.LastName = r.LastName;
            builder.Email = r.Email;
            builder.Password = r.Password;
            builder.Role = r.Role;
        },
        userEfRepository).ConfigureAwait(false);
    }

    public static CreateUserResponse FromEntity(User e)
    {
        return new CreateUserResponse
        (
            Id: e.Id,
            FirstName: e.FirstName,
            LastName: e.LastName,
            Email: e.Email.Value,
            Role: e.Role.Value
        );
    }
}