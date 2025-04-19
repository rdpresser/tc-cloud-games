namespace TC.CloudGames.Application.Email
{
    public interface IEmailService
    {
        Task SendAsync(Domain.User.Email recipient, string subject, string body);
    }
}
