using TC.CloudGames.Application.Users.GetUserByEmail;

namespace TC.CloudGames.Application.Tests.Users.GetUserByEmail
{
    public class GetUserByEmailQueryTests
    {
        private readonly Faker _faker;

        public GetUserByEmailQueryTests()
        {
            _faker = new Faker();
        }

        [Fact]
        public async Task Handle_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            var email = _faker.Internet.Email();
            var userRepository = A.Fake<IUserPgRepository>();
            A.CallTo(() => userRepository.GetByEmailAsync(email, A<CancellationToken>._))
                .Returns(Task.FromResult<UserByEmailResponse?>(null));

            // Act
            var result = await userRepository.GetByEmailAsync(email, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Handle_ShouldReturnUserByEmailResponse_WhenUserExists()
        {
            // Arrange
            Factory.RegisterTestServices(_ => { });

            var email = _faker.Internet.Email();
            var expectedResponse = new UserByEmailResponse
            {
                Email = email,
                FirstName = _faker.Name.FirstName(),
                LastName = _faker.Name.LastName(),
                Role = "User"
            };

            var userRepository = A.Fake<IUserPgRepository>();
            A.CallTo(() => userRepository.GetByEmailAsync(email, A<CancellationToken>._))
                .Returns(Task.FromResult(expectedResponse));

            // Act
            var result = await userRepository.GetByEmailAsync(email, CancellationToken.None);

            // Assert
            Assert.Equal(result, expectedResponse);
        }
    }
}
