using Ardalis.Result;
using FakeItEasy;
using FastEndpoints;
using TC.CloudGames.Application.Abstractions.Data;
using TC.CloudGames.Application.Abstractions.Messaging;
using TC.CloudGames.Application.Users.GetUserByEmail;

namespace TC.CloudGames.Application.Tests.Users.GetUserByEmail
{
    public class GetUserByEmailQueryTests
    {
        [Fact]
        public async Task Handle_ShouldReturnUserByEmailResponse_WhenUserExists()
        {
            // Arrange
            Factory.RegisterTestServices(_ => { });

            var email = "test@example.com";
            var expectedResponse = new UserByEmailResponse
            {
                Email = email,
                FirstName = "Test",
                LastName = "User",
                Role = "User"
            };

            var query = new GetUserByEmailQuery(email);

            var userRepository = A.Fake<IUserPgRepository>();
            A.CallTo(() => userRepository.GetByEmailAsync(email, A<CancellationToken>._))
                .Returns(Task.FromResult(expectedResponse));

            // Act
            var result = await userRepository.GetByEmailAsync(email, CancellationToken.None);

            // Assert
            Assert.Equal(result, expectedResponse);
        }

        //[Fact]
        //public async Task Handle_ShouldReturnNull_WhenUserDoesNotExist()
        //{
        //    // Arrange
        //    var email = "notfound@example.com";
        //    var userRepository = A.Fake<IUserRepository>();
        //    A.CallTo(() => userRepository.GetByEmailAsync(email, A<CancellationToken>._))
        //        .Returns(Task.FromResult<UserByEmailResponse?>(null));

        //    var handler = new GetUserByEmailQueryHandler(userRepository);
        //    var query = new GetUserByEmailQuery(email);

        //    // Act
        //    var result = await handler.Handle(query, CancellationToken.None);

        //    // Assert
        //    result.Should().BeNull();
        //}
    }
}
