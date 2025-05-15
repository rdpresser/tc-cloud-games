using Ardalis.Result;
using TC.CloudGames.Domain.User;
using TC.CloudGames.Domain.User.Abstractions;

namespace TC.CloudGames.Application.Users.CreateUser;

public static class CreateUserMapper
{
    public static async Task<Result<User>> ToEntityAsync(CreateUserCommand r, IUserEfRepository userEfRepository)
    {
        return await User.CreateAsync(
            firstName: r.FirstName,
            lastName: r.LastName,
            email: r.Email,
            password: r.Password,
            role: r.Role,
            userEfRepository
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