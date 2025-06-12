namespace TC.CloudGames.Application.Abstractions.Email
{
    public interface IEmailService
    {
        Task SendAsync(Domain.Aggregates.User.ValueObjects.Email recipient, string subject, string body);
    }
}
