using Ardalis.Result;
using TC.CloudGames.Domain.User;
using TC.CloudGames.Infra.CrossCutting.Commons.Authentication;

namespace TC.CloudGames.Application.Users.CreateUser;

public static class CreateUserMapper
{
    public static Result<User> ToEntity(CreateUserCommand r, IPasswordHasher passwordHasher)
    {
        List<ValidationError> validation = [];

        var emailResult = Email.Create(r.Email);
        if (!emailResult.IsSuccess)
        {
            validation.AddRange(emailResult.ValidationErrors);
        }

        var passwordResult = Password.Create(r.Password);
        if (!passwordResult.IsSuccess)
        {
            validation.AddRange(passwordResult.ValidationErrors);
        }

        var roleResult = Role.Create(r.Role);
        if (!roleResult.IsSuccess)
        {
            validation.AddRange(roleResult.ValidationErrors);
        }

        var userResult = User.Create(
            firstName: r.FirstName,
            lastName: r.LastName,
            email: emailResult,
            password: passwordResult,
            role: roleResult
        );

        if (!userResult.IsSuccess)
        {
            validation.AddRange(userResult.ValidationErrors);
        }

        if (validation.Count != 0)
        {
            return Result<User>.Invalid(validation);
        }

        return userResult;

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