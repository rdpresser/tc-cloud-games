namespace TC.CloudGames.Application.Users.CreateUser
{
    public interface ICreateUserApp
    {
        Task<CreateUserResponse> InvokeAsync(CreateUserRequest request, CancellationToken ct);
    }
}
