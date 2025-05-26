using FakeItEasy;
using FastEndpoints;
using TC.CloudGames.Application.Abstractions;
using TC.CloudGames.Application.Abstractions.Data;
using TC.CloudGames.Application.Users.GetUserById;
using TC.CloudGames.Infra.CrossCutting.Commons.Authentication;

namespace TC.CloudGames.Application.Tests.Users.GetUserById
{
    public class GetUserByIdQueryHandlerTests
    {
        [Fact]
        public async Task ExecuteAsync_ShouldReturnUser_WhenUserExistsAndAuthorized()
        {
            // Arrange
            Factory.RegisterTestServices(_ => { });

            var userRepository = A.Fake<IUserPgRepository>();
            var userContext = A.Fake<IUserContext>();
            var handler = new GetUserByIdQueryHandler(userRepository, userContext);

            var userId = Guid.NewGuid();
            var query = new GetUserByIdQuery(userId);
            var userResponse = new UserByIdResponse
            {
                Id = userId,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@email.com",
                Role = "Admin"
            };

            A.CallTo(() => userContext.UserRole).Returns("Admin");
            A.CallTo(() => userRepository.GetByIdAsync(userId, A<CancellationToken>._)).Returns(userResponse);

            // Act
            var result = await handler.ExecuteAsync(query);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(userResponse, result.Value);
        }

        //[Fact]
        //public async Task ExecuteAsync_ShouldReturnNotAuthorized_WhenUserIsNotAdminAndTriesToAccessOtherUser()
        //{
        //    // Arrange
        //    Factory.RegisterTestServices(_ => { });

        //    var userRepository = A.Fake<IUserPgRepository>();
        //    var userContext = A.Fake<IUserContext>();
        //    var handler = new GetUserByIdQueryHandler(userRepository, userContext);

        //    var userId = Guid.NewGuid();
        //    var query = new GetUserByIdQuery(userId);

        //    A.CallTo(() => userContext.UserRole).Returns(AppConstants.UserRole);
        //    A.CallTo(() => userContext.UserId).Returns(Guid.NewGuid()); // Different from query.Id

        //    // Act
        //    var result = await handler.ExecuteAsync(query);

        //    // Assert
        //    Assert.False(result.IsSuccess);
        //    Assert.Single(result.Errors, e => e.ErrorCode == $"{nameof(GetUserByIdQuery.Id)}.NotAuthorized");
        //}

        //[Fact]
        //public async Task ExecuteAsync_ShouldReturnUser_WhenUserIsSelf()
        //{
        //    // Arrange
        //    var userId = Guid.NewGuid();
        //    var query = new GetUserByIdQuery(userId);
        //    var userResponse = new UserByIdResponse(userId, "Jane", "Smith", "jane.smith@email.com", AppConstants.UserRole);

        //    A.CallTo(() => _userContext.UserRole).Returns(AppConstants.UserRole);
        //    A.CallTo(() => _userContext.UserId).Returns(userId);
        //    A.CallTo(() => _userRepository.GetByIdAsync(userId, A<CancellationToken>._)).Returns(userResponse);

        //    // Act
        //    var result = await _handler.ExecuteAsync(query);

        //    // Assert
        //    result.IsSuccess.Should().BeTrue();
        //    result.Value.Should().BeEquivalentTo(userResponse);
        //}

        //[Fact]
        //public async Task ExecuteAsync_ShouldReturnNotFound_WhenUserDoesNotExist()
        //{
        //    // Arrange
        //    var userId = Guid.NewGuid();
        //    var query = new GetUserByIdQuery(userId);

        //    A.CallTo(() => _userContext.UserRole).Returns("Admin");
        //    A.CallTo(() => _userRepository.GetByIdAsync(userId, A<CancellationToken>._)).Returns((UserByIdResponse?)null);

        //    // Act
        //    var result = await _handler.ExecuteAsync(query);

        //    // Assert
        //    result.IsSuccess.Should().BeFalse();
        //    result.Errors.Should().ContainSingle(e => e.ErrorCode == Domain.User.Abstractions.UserDomainErrors.NotFound.ErrorCode);
        //}
    }
}
