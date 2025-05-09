namespace TC.CloudGames.Infra.CrossCutting.Commons.Authentication
{
    public interface IUserContext
    {
        Guid UserId { get; }
        string UserEmail { get; }
        string UserName { get; }
        string UserRole { get; }
    }
}
