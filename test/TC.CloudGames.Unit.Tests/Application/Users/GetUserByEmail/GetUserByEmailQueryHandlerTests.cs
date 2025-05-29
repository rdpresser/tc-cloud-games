using TC.CloudGames.Application.Abstractions;
using TC.CloudGames.Application.Users.GetUserByEmail;
using TC.CloudGames.Infra.CrossCutting.Commons.Authentication;

namespace TC.CloudGames.Unit.Tests.Application.Users.GetUserByEmail
{
    public class GetUserByEmailQueryHandlerTests
    {
        private readonly Faker _faker;

        public GetUserByEmailQueryHandlerTests()
        {
            _faker = new Faker();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnUser_WhenUserExistsAndAuthorized()
        {
            // Arrange
            Factory.RegisterTestServices(_ => { });

            var userContext = A.Fake<IUserContext>();
            var userRepository = A.Fake<IUserPgRepository>();

            var email = _faker.Internet.Email();
            var query = new GetUserByEmailQuery(email);
            var userResponse = new UserByEmailResponse
            {
                Email = email,
                FirstName = _faker.Name.FirstName(),
                LastName = _faker.Name.LastName(),
                Role = "User"
            };

            A.CallTo(() => userContext.UserRole).Returns(AppConstants.UserRole);
            A.CallTo(() => userContext.UserEmail).Returns(email);
            A.CallTo(() => userRepository.GetByEmailAsync(email, A<CancellationToken>._)).Returns(userResponse);

            var fakeHandler = A.Fake<QueryHandler<GetUserByEmailQuery, UserByEmailResponse>>();
            A.CallTo(() => fakeHandler.ExecuteAsync(A<GetUserByEmailQuery>.Ignored, A<CancellationToken>.Ignored))
                .Returns(Task.FromResult(Result<UserByEmailResponse>.Success(userResponse)));

            // Act
            var result = await fakeHandler.ExecuteAsync(query, TestContext.Current.CancellationToken);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(userResponse, result.Value);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnNotAuthorized_WhenUserTriesToAccessOtherUser()
        {
            // Arrange
            Factory.RegisterTestServices(_ => { });

            var userContext = A.Fake<IUserContext>();

            var email = "other@example.com";
            var query = new GetUserByEmailQuery(email);

            A.CallTo(() => userContext.UserRole).Returns(AppConstants.UserRole);
            A.CallTo(() => userContext.UserEmail).Returns("test@example.com");

            var fakeHandler = A.Fake<QueryHandler<GetUserByEmailQuery, UserByEmailResponse>>();
            var notAuthorizedResult = Result<UserByEmailResponse>.Invalid(
                new ValidationError
                {
                    ErrorMessage = "You are not authorized to access this user.",
                    ErrorCode = $"{nameof(GetUserByEmailQuery.Email)}.NotAuthorized"
                }
            );

            A.CallTo(() => fakeHandler.ExecuteAsync(A<GetUserByEmailQuery>.Ignored, A<CancellationToken>.Ignored))
                .Returns(Task.FromResult(notAuthorizedResult));

            // Act
            var result = await fakeHandler.ExecuteAsync(query, TestContext.Current.CancellationToken);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains(result.ValidationErrors, e => e.ErrorCode == $"{nameof(GetUserByEmailQuery.Email)}.NotAuthorized");
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            Factory.RegisterTestServices(_ => { });

            var userContext = A.Fake<IUserContext>();

            var email = _faker.Internet.Email();
            var query = new GetUserByEmailQuery(email);
            var userResponse = new UserByEmailResponse
            {
                Email = email,
                FirstName = _faker.Name.FirstName(),
                LastName = _faker.Name.LastName(),
                Role = "User"
            };

            A.CallTo(() => userContext.UserRole).Returns(AppConstants.UserRole);
            A.CallTo(() => userContext.UserEmail).Returns("test@example.com");

            var fakeHandler = A.Fake<QueryHandler<GetUserByEmailQuery, UserByEmailResponse>>();
            var notAuthorizedResult = Result<UserByEmailResponse>.Invalid(
                new ValidationError
                {
                    ErrorMessage = $"User with email '{userResponse.Email}' not found.",
                    ErrorCode = $"{nameof(GetUserByEmailQuery.Email)}.NotFound"
                }
            );

            A.CallTo(() => fakeHandler.ExecuteAsync(A<GetUserByEmailQuery>.Ignored, A<CancellationToken>.Ignored))
                .Returns(Task.FromResult(notAuthorizedResult));

            // Act
            var result = await fakeHandler.ExecuteAsync(query, TestContext.Current.CancellationToken);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains(result.ValidationErrors, e => e.ErrorCode == $"{nameof(GetUserByEmailQuery.Email)}.NotFound");

        }

        [Fact]
        public void Constructor_WithValidDependencies_ShouldNotThrow()
        {
            // Arrange
            Factory.RegisterTestServices(_ => { });

            var userRepository = A.Fake<IUserPgRepository>();
            var userContext = A.Fake<IUserContext>();

            // Act & Assert
            var handler = new GetUserByEmailQueryHandler(userRepository, userContext);
            Assert.NotNull(handler);
        }

        [Fact]
        public void Constructor_WithNullUserRepository_ShouldThrowArgumentNullException()
        {
            // Arrange
            Factory.RegisterTestServices(_ => { });

            IUserPgRepository userRepository = null!;
            var userContext = A.Fake<IUserContext>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new GetUserByEmailQueryHandler(userRepository, userContext));
        }

        [Fact]
        public void Constructor_WithNullUserContext_ShouldThrowArgumentNullException()
        {
            // Arrange
            Factory.RegisterTestServices(_ => { });

            var userRepository = A.Fake<IUserPgRepository>();
            IUserContext userContext = null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new GetUserByEmailQueryHandler(userRepository, userContext));
        }

    }
}
