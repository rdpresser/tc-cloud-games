using TC.CloudGames.Api.Extensions;

namespace TC.CloudGames.Api.Tests.Extensions
{
    public class PasswordGeneratorTests(App App) : TestBase<App>
    {
        [Fact]
        public void GeneratePassword_ValidLength_ReturnsPassword()
        {
            // Arrange
            int length = 12; // Example length
            string password = PasswordGenerator.GeneratePassword(length);
            // Act & Assert
            password.Length.ShouldBe(length);
            password.ShouldContain(c => char.IsUpper(c));
            password.ShouldContain(c => char.IsLower(c));
            password.ShouldContain(c => char.IsDigit(c));
            password.ShouldContain(c => "!@#$%^&*()_+-=[]{}|;:,.<>?".Contains(c));
        }

        [Fact]
        public void GeneratePassword_InvalidLength_ThrowsException()
        {
            // Arrange
            int length = 5; // Invalid length
            // Act & Assert
            Should.Throw<ArgumentException>(() => PasswordGenerator.GeneratePassword(length))
                .Message.ShouldBe("Password length must be at least 8 characters.");
        }
    }
}
