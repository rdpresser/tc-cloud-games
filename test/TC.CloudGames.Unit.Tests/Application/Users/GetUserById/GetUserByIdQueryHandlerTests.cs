using TC.CloudGames.Application.Abstractions;
using TC.CloudGames.Application.Users.GetUserById;
using TC.CloudGames.Infra.CrossCutting.Commons.Authentication;
using TC.CloudGames.Unit.Tests.Fakes;

namespace TC.CloudGames.Unit.Tests.Application.Users.GetUserById
{
    public class GetUserByIdQueryHandlerTests
    {
        [Fact]
        public async Task ExecuteAsync_ShouldReturnUser_WhenUserIsSelf()
        {
            // Arrange
            Factory.RegisterTestServices(_ => { });

            var userValid = FakeUserData.UserValid();
                
            var userRepository = A.Fake<IUserPgRepository>();
            var userContext = A.Fake<IUserContext>();
            var handler = new GetUserByIdQueryHandler(userRepository, userContext);

            var userId = Guid.NewGuid();
            var query = new GetUserByIdQuery(userId);
            var userResponse = new UserByIdResponse
            {
                Id = userId,
                FirstName = userValid.FistName,
                LastName = userValid.LastName,
                Email = userValid.Email,
                Role = AppConstants.UserRole
            };

            A.CallTo(() => userContext.UserRole).Returns(AppConstants.UserRole);
            A.CallTo(() => userContext.UserId).Returns(userId);
            A.CallTo(() => userRepository.GetByIdAsync(userId, A<CancellationToken>._)).Returns(userResponse);

            // Act
            var result = await handler.ExecuteAsync(query, TestContext.Current.CancellationToken);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(userResponse, result.Value);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnUser_WhenUserExistsAndAuthorized()
        {
            // Arrange
            Factory.RegisterTestServices(_ => { });

            var userValid = FakeUserData.UserValid();
            
            var userRepository = A.Fake<IUserPgRepository>();
            var userContext = A.Fake<IUserContext>();
            var handler = new GetUserByIdQueryHandler(userRepository, userContext);

            var userId = Guid.NewGuid();
            var query = new GetUserByIdQuery(userId);
            var userResponse = new UserByIdResponse
            {
                Id = userId,
                FirstName = userValid.FistName,
                LastName = userValid.LastName,
                Email = userValid.Email,
                Role = "Admin"
            };

            A.CallTo(() => userContext.UserRole).Returns("Admin");
            A.CallTo(() => userRepository.GetByIdAsync(userId, A<CancellationToken>._)).Returns(userResponse);

            // Act
            var result = await handler.ExecuteAsync(query, TestContext.Current.CancellationToken);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(userResponse, result.Value);
        }
    }
}
