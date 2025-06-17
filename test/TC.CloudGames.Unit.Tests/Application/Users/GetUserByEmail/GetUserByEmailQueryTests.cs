using TC.CloudGames.Application.Users.GetUserByEmail;
using TC.CloudGames.Unit.Tests.Fakes;

namespace TC.CloudGames.Unit.Tests.Application.Users.GetUserByEmail
{
    public class GetUserByEmailQueryTests
    {
        [Fact]
        public async Task Handle_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            var userValid = FakeUserData.UserValid();
            
            var email = userValid.Email;
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

            var userValid = FakeUserData.UserValid();
            
            var email = userValid.Email;
            var expectedResponse = new UserByEmailResponse
            {
                Email = email,
                FirstName = userValid.FistName,
                LastName = userValid.LastName,
                Role = userValid.Role
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
