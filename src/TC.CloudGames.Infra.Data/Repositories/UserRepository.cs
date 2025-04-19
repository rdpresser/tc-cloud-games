using TC.CloudGames.Domain.User;

namespace TC.CloudGames.Infra.Data.Repositories
{
    public sealed class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
