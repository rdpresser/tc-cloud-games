namespace TC.CloudGames.Application.Abstractions.Email
{
    public interface IEmailService
    {
        Task SendAsync(Domain.UserAggregate.ValueObjects.Email recipient, string subject, string body);
    }
}
