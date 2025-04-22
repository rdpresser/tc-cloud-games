namespace TC.CloudGames.Application.Users.GetUserList
{
    public sealed class UserListResponse
    {
        public Guid Id { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Email { get; init; }
        public string Role { get; init; }
    }
}
