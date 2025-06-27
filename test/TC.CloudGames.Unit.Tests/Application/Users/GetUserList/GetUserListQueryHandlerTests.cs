using TC.CloudGames.Application.Users.GetUserList;

namespace TC.CloudGames.Unit.Tests.Application.Users.GetUserList
{
    public class GetUserListQueryHandlerTests
    {
        [Fact]
        public async Task ExecuteAsync_ShouldReturnEmptyList_WhenNoUsersFound()
        {
            // Arrange
            Factory.RegisterTestServices(_ => { });

            var userRepository = A.Fake<IUserPgRepository>();
            var handler = new GetUserListQueryHandler(userRepository);

            var query = new GetUserListQuery();
            A.CallTo(() => userRepository.GetUserListAsync(query, A<CancellationToken>._))
                .Returns(Task.FromResult<IReadOnlyList<UserListResponse>>(Array.Empty<UserListResponse>()));

            // Act
            var result = await handler.ExecuteAsync(query, TestContext.Current.CancellationToken);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Empty(result.Value);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnUserList_WhenUsersExist()
        {
            // Arrange
            Factory.RegisterTestServices(_ => { });

            var userRepository = A.Fake<IUserPgRepository>();
            var handler = new GetUserListQueryHandler(userRepository);

            var query = new GetUserListQuery();
            var users = new List<UserListResponse>
            {
                new UserListResponse(/* fill with test data if needed */),
                new UserListResponse(/* fill with test data if needed */)
            };
            A.CallTo(() => userRepository.GetUserListAsync(query, A<CancellationToken>._))
                .Returns(Task.FromResult<IReadOnlyList<UserListResponse>>(users));

            // Act
            var result = await handler.ExecuteAsync(query, TestContext.Current.CancellationToken);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(2, result.Value.Count);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnEmptyList_WhenRepositoryReturnsNull()
        {
            // Arrange
            Factory.RegisterTestServices(_ => { });

            var userRepository = A.Fake<IUserPgRepository>();
            var handler = new GetUserListQueryHandler(userRepository);

            var query = new GetUserListQuery();
            A.CallTo(() => userRepository.GetUserListAsync(query, A<CancellationToken>._))
                .Returns(Task.FromResult<IReadOnlyList<UserListResponse>>(null!)); // Return null explicitly

            // Act
            var result = await handler.ExecuteAsync(query, TestContext.Current.CancellationToken);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Empty(result.Value);
        }
    }
}
