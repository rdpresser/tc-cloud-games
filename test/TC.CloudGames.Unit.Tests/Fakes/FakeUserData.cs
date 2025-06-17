using TC.CloudGames.Unit.Tests.FakeModels;

namespace TC.CloudGames.Unit.Tests.Fakes;

public static class FakeUserData
{
    public static User UserValid()
    {
        return new User
        {
            FistName = "John",
            LastName = "Doe",
            Email = "john.doe@email.com",
            Password = "SecurePassword@123",
            Role = "User"
        };
    }
}