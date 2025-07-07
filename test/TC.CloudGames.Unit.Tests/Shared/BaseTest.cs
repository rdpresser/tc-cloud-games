using FluentValidation;
using FluentValidation.Resources;

namespace TC.CloudGames.Unit.Tests.Shared
{
    public class BaseTest : TestBase
    {
        protected BaseTest()
        {
            ValidatorOptions.Global.PropertyNameResolver = (type, memberInfo, expression) => memberInfo?.Name;
            ValidatorOptions.Global.DisplayNameResolver = (type, memberInfo, expression) => memberInfo?.Name;
            ValidatorOptions.Global.ErrorCodeResolver = validator => validator.Name;
            ValidatorOptions.Global.LanguageManager = new LanguageManager
            {
                Enabled = true,
                Culture = new System.Globalization.CultureInfo("en")
            };
        }

        protected void LogTestStart(string testName) => Output.WriteLine($"Starting test: {testName}");

        protected string CreateValidEmail => Fake.Internet.Email();
        protected string CreateValidPassword => Fake.Internet.Password(12, true, "\\w", "!A1a");
        protected static string ValidFirstName => "John";
        protected static string ValidLastName => "Doe";

        protected void PrintValidationErrors(IEnumerable<FluentValidation.Results.ValidationFailure> errors)
        {
            Output.WriteLine("-------------------------------------------------------------------------------");
            foreach (var error in errors)
            {
                Output.WriteLine($"EXPECTED ERROR => PropertyName: {error.PropertyName} | ErrorMessage: {error.ErrorMessage} | ErrorCode: {error.ErrorCode}");
            }
            Output.WriteLine("-------------------------------------------------------------------------------");
        }

        protected void PrintValidationErrors(IEnumerable<ValidationError> errors)
        {
            Output.WriteLine("-------------------------------------------------------------------------------");
            foreach (var error in errors)
            {
                Output.WriteLine($"EXPECTED ERROR => Identifier: {error.Identifier} | ErrorMessage: {error.ErrorMessage} | ErrorCode: {error.ErrorCode}");
            }
            Output.WriteLine("-------------------------------------------------------------------------------");
        }

        protected static IEnumerable<(string Identifier, int Count, IEnumerable<string> ErrorCodes)> GroupValidationErrorsByIdentifier(IEnumerable<ValidationError> errors)
        {
            return errors
                .GroupBy(e => e.Identifier)
                .Select(g => (
                    Identifier: g.Key,
                    Count: g.Count(),
                    ErrorCodes: g.Select(e => $"{e.ErrorCode} - {e.ErrorMessage}")
                ));
        }
    }
}
