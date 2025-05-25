using FakeItEasy;
using FastEndpoints;
using TC.CloudGames.Application.Abstractions.Data;
using TC.CloudGames.Application.Users.CreateUser;
using TC.CloudGames.Domain.User;
using TC.CloudGames.Domain.User.Abstractions;

namespace TC.CloudGames.Application.Tests.Users.CreateUser
{
    public class CreateUserCommandHandlerTests
    {

        [Fact]
        public async Task ExecuteAsync_ShouldReturnInvalid_WhenEntityCreationFails()
        {
            // Arrange
            Factory.RegisterTestServices(_ => { });

            var unitOfWorkFake = A.Fake<IUnitOfWork>();
            var repositoryFake = A.Fake<IUserEfRepository>();

            var command = new CreateUserCommand(
                FirstName: "Test",
                LastName: "User",
                Email: "email_invalido",
                Password: "Test@1234",
                Role: "User"
            );

            var handler = new CreateUserCommandHandler(unitOfWorkFake, repositoryFake);

            // Act
            var result = await handler.ExecuteAsync(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains(result.ValidationErrors, e => e.Identifier == "Email");
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnInvalid_WhenDuplicateKeyExceptionIsThrown()
        {
            // Arrange
            Factory.RegisterTestServices(_ => { });

            var unitOfWorkFake = A.Fake<IUnitOfWork>();
            var repositoryFake = A.Fake<IUserEfRepository>();

            var command = new CreateUserCommand(
                FirstName: "Test",
                LastName: "User",
                Email: "duplicate@email.com",
                Password: "Test@1234",
                Role: "User"
            );

            A.CallTo(() => repositoryFake.Add(A<User>.Ignored))
                .Throws(new DuplicateKeyViolationException("Duplicate key", "Users", "Email"));

            var handler = new CreateUserCommandHandler(unitOfWorkFake, repositoryFake);

            // Act
            var result = await handler.ExecuteAsync(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains(result.ValidationErrors, e => e.Identifier == "Email" || e.ErrorMessage.Contains("duplicate", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnSuccess_WhenUserIsCreatedCorrectly()
        {
            // Arrange
            Factory.RegisterTestServices(_ => { });

            var unitOfWorkFake = A.Fake<IUnitOfWork>();
            var repositoryFake = A.Fake<IUserEfRepository>();

            var command = new CreateUserCommand(
                FirstName: "Valid",
                LastName: "User",
                Email: "validuser@email.com",
                Password: "Valid@1234",
                Role: "User"
            );

            User? createdUser = null;
            A.CallTo(() => repositoryFake.Add(A<User>.Ignored))
                .Invokes((User u) => createdUser = u);

            var handler = new CreateUserCommandHandler(unitOfWorkFake, repositoryFake);

            // Act
            var result = await handler.ExecuteAsync(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(command.FirstName, createdUser.FirstName);
            Assert.Equal(command.LastName, createdUser.LastName);
            Assert.Equal(command.Email, createdUser.Email.Value);
            Assert.Equal(command.Role, createdUser.Role.Value);
            A.CallTo(() => repositoryFake.Add(A<User>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => unitOfWorkFake.SaveChangesAsync(A<CancellationToken>.Ignored)).MustHaveHappenedOnceExactly();
        }

    }
}
