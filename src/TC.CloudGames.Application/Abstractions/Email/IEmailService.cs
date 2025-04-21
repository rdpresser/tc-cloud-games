namespace TC.CloudGames.Application.Abstractions.Email
{
    public interface IEmailService
    {
        Task SendAsync(Domain.User.Email recipient, string subject, string body);
    }
}
