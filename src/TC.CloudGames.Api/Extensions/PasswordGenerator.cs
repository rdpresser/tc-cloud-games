using System.Text;

namespace TC.CloudGames.Api.Extensions
{
    /// <summary>
    /// Provides functionality to generate secure random passwords that meet specific complexity requirements.
    /// </summary>
    /// <remarks>
    /// The generated password will always include at least one uppercase letter, one lowercase letter, 
    /// one digit, and one special character to ensure compliance with common password policies. 
    /// The remaining characters are randomly selected from a combination of uppercase, lowercase, digits, 
    /// and special characters. The password is then shuffled to ensure randomness.
    /// </remarks>
    public static class PasswordGenerator
    {
        private static Random random = new Random();

        public static string GeneratePassword(int length = 8)
        {
            if (length < 8)
            {
                throw new ArgumentException("Password length must be at least 8 characters.");
            }

            const string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowerCase = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string specialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";

            StringBuilder password = new StringBuilder();

            // Ensure each constraint is met
            password.Append(upperCase[random.Next(upperCase.Length)]);
            password.Append(lowerCase[random.Next(lowerCase.Length)]);
            password.Append(digits[random.Next(digits.Length)]);
            password.Append(specialChars[random.Next(specialChars.Length)]);

            // Fill the rest of the password length with random characters from all sets
            string allChars = upperCase + lowerCase + digits + specialChars;
            for (int i = password.Length; i < length; i++)
            {
                password.Append(allChars[random.Next(allChars.Length)]);
            }

            // Shuffle the characters in the password to ensure randomness
            return new string(password.ToString().OrderBy(c => random.Next()).ToArray());
        }
    }
}