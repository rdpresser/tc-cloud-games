using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using TC.CloudGames.Application.Users.GetUserByEmail;
using TC.CloudGames.Application.Users.Login;
using TC.CloudGames.Integration.Tests.Abstractions;

namespace TC.CloudGames.Integration.Tests.Auth
{
    public class LoginEndpointIntegrationTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
    {
        private readonly IntegrationTestWebAppFactory _factory = factory;

        //[Fact]
        public async Task Login_ValidCredentials_ReturnsSuccess()
        {
            // Arrange
            var loginReq = new LoginUserCommand(Email: "user@user.com", Password: "User@123");
            var userByEmailReq = new GetUserByEmailQuery(loginReq.Email);
            var userByEmailRes = new UserByEmailResponse
            {
                FirstName = "Regular",
                LastName = "User",
                Email = loginReq.Email,
                Role = "User"
            };

            // Act
            var client = _factory.CreateClient();
            var (rsp, res) = await _factory.DoLogin(client, loginReq.Email, loginReq.Password);

            var resUserByEmail = await client.GetFromJsonAsync<UserByEmailResponse>($"api/user/by-email/{userByEmailReq.Email}",
                TestContext.Current.CancellationToken);

            // Assert
            rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
            resUserByEmail!.Email.ShouldBe(userByEmailReq.Email, StringCompareShould.IgnoreCase);
            res.Email.ShouldBe(loginReq.Email, StringCompareShould.IgnoreCase);
            resUserByEmail.Email.ShouldBe(loginReq.Email, StringCompareShould.IgnoreCase);

            // Assert: Token is not null/empty and has JWT format
            res.JwtToken.ShouldNotBeNullOrWhiteSpace();
            res.JwtToken.Split('.').Length.ShouldBe(3);

            // Assert: Decode and validate JWT
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(res.JwtToken);

            // Check claims
            var name = $"{userByEmailRes.FirstName} {userByEmailRes.LastName}";
            jwt.Claims.First(c => c.Type == "sub").Value.ShouldBe(resUserByEmail.Id.ToString(), StringCompareShould.IgnoreCase);
            jwt.Claims.First(c => c.Type == "name").Value.ShouldBe(name, StringCompareShould.IgnoreCase);
            jwt.Claims.First(c => c.Type == "email").Value.ShouldBe(loginReq.Email, StringCompareShould.IgnoreCase);
            jwt.Claims.First(c => c.Type == "role").Value.ShouldBe(userByEmailRes.Role, StringCompareShould.IgnoreCase);

            // Optionally, validate signature and expiration
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _factory.JwtAppSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _factory.JwtAppSettings.Audience,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_factory.JwtAppSettings.SecretKey))
            };

            handler.ValidateToken(res.JwtToken, validationParameters, out var validatedToken);
            validatedToken.ShouldNotBeNull();
        }

        //[Fact]
        //public async Task Login_InvalidCredentials_ReturnsNotFound()
        //{
        //    // Arrange
        //    var loginReq = new LoginUserCommand(Email: "user@user.com", Password: "Invalid1_password!");

        //    // Act
        //    // Attempt to login with invalid credentials
        //    var (rsp, res) = await App.GuestClient
        //        .POSTAsync<LoginEndpoint, LoginUserCommand, LoginUserResponse>(loginReq);

        //    // Assert
        //    rsp.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        //    res.Email.ShouldBeNull();
        //}

        //[Fact]
        //public async Task Login_InvalidCredentials_ReturnsBadRequest()
        //{
        //    // Arrange
        //    var loginReq = new LoginUserCommand(Email: "user@user.com", Password: "Invalid_password");

        //    // Act
        //    // Attempt to login with invalid credentials
        //    var (rsp, res) = await App.GuestClient
        //        .POSTAsync<LoginEndpoint, LoginUserCommand, LoginUserResponse>(loginReq);

        //    // Assert
        //    rsp.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        //    res.Email.ShouldBeNull();
        //}
    }
}
