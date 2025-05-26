using TC.CloudGames.Api.Endpoints.Auth;
using TC.CloudGames.Api.Tests.Abstractions;
using TC.CloudGames.Application.Users.CreateUser;
using TC.CloudGames.Domain.User.Abstractions;

namespace TC.CloudGames.Api.Tests.Endpoints.Auth
{
    public class CreateUserEndpointTests : TestBase<App>
    {
        [Fact]
        public async Task CreateUser_ValidData_ReturnsSuccess()
        {
            // Arrange
            var ep = Factory.Create<CreateUserEndpoint>();
            var createUserReq = new CreateUserCommand
            (
                FirstName: "John",
                LastName: "Doe",
                Email: "john.doe@test.com",
                Password: "Password123!",
                Role: "User"
            );

            var createUserRes = new CreateUserResponse
            (
                Id: Guid.NewGuid(),
                FirstName: createUserReq.FirstName,
                LastName: createUserReq.LastName,
                Email: createUserReq.Email,
                Role: createUserReq.Role
            );

            var fakeHandler = A.Fake<IAppCommandHandler.CommandHandler<CreateUserCommand, CreateUserResponse, Domain.User.User, IUserEfRepository>>();
            A.CallTo(() => fakeHandler.ExecuteAsync(A<CreateUserCommand>.Ignored, A<CancellationToken>.Ignored))
                .Returns(Task.FromResult(Result<CreateUserResponse>.Success(createUserRes)));

            fakeHandler.RegisterForTesting();

            // Act
            await ep.HandleAsync(createUserReq, TestContext.Current.CancellationToken);

            // Assert
            ep.Response.Id.ShouldBe(createUserRes.Id);
            ep.Response.FirstName.ShouldBe(createUserRes.FirstName);
            ep.Response.LastName.ShouldBe(createUserRes.LastName);
            ep.Response.Email.ShouldBe(createUserRes.Email);
            ep.Response.Role.ShouldBe(createUserRes.Role);

            // Additional Assertions
            var result = await fakeHandler.ExecuteAsync(createUserReq, CancellationToken.None);
            result.IsSuccess.ShouldBeTrue();
            result.IsInvalid().ShouldBeFalse();
            result.ValidationErrors.Count().ShouldBe(0);
            result.Value.ShouldNotBeNull();
            result.Errors.Count().ShouldBe(0);
        }

        [Fact]
        public async Task CreateUser_InvalidData_ReturnsFailure()
        {
            // Arrange
            var ep = Factory.Create<CreateUserEndpoint>();
            var createUserReq = new CreateUserCommand
            (
                FirstName: "John",
                LastName: "Doe",
                Email: "invalid-email",
                Password: "short",
                Role: "InvalidRole"
            );

            var listError = new List<ValidationError>
            {
                new() {
                    Identifier = "Password",
                    ErrorMessage = "Password must be at least 8 characters long.",
                    ErrorCode = "Password.MinimumLength"
                },
                new() {
                    Identifier = "Role",
                    ErrorMessage = "Invalid role specified.",
                    ErrorCode = "Role.InvalidRole"
                },
                new() {
                    Identifier = "Email",
                    ErrorMessage = "Invalid email format.",
                    ErrorCode = "Email.InvalidFormat"
                }
            };

            var fakeHandler = A.Fake<IAppCommandHandler.CommandHandler<CreateUserCommand, CreateUserResponse, Domain.User.User, IUserEfRepository>>();
            A.CallTo(() => fakeHandler.ExecuteAsync(A<CreateUserCommand>.Ignored, A<CancellationToken>.Ignored))
                .Returns(Task.FromResult(Result<CreateUserResponse>.Invalid(listError)));
            fakeHandler.RegisterForTesting();

            // Act
            await ep.HandleAsync(createUserReq, TestContext.Current.CancellationToken);

            // Assert
            ep.Response.Id.ShouldBe(Guid.Empty);
            ep.Response.FirstName.ShouldBeNull();
            ep.Response.LastName.ShouldBeNull();
            ep.Response.Email.ShouldBeNull();
            ep.Response.Role.ShouldBeNull();

            // Additional Assertions
            var result = await fakeHandler.ExecuteAsync(createUserReq, CancellationToken.None);
            result.IsSuccess.ShouldBeFalse();
            result.IsInvalid().ShouldBeTrue();
            result.ValidationErrors.Count().ShouldBe(3);
            result.Value.ShouldBeNull();
            result.Errors.Count().ShouldBe(0);
        }
    }
}
