using Microsoft.EntityFrameworkCore;
using TC.CloudGames.Domain.User;

namespace TC.CloudGames.Infra.Data.Repositories
{
    public sealed class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext dbContext)
            : base(dbContext)
        {

        }

        public async Task<User?> GetByEmailWithPasswordAsync(string email, string password,
            CancellationToken cancellationToken = default)
        {
            return await DbContext
                .Users
                .FirstOrDefaultAsync(entity =>
                    entity.Email == Email.Create(email) &&
                    entity.Password == Password.Create(password), cancellationToken).ConfigureAwait(false);
        }

        public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
        {
            return await DbContext
                .Users
                .AnyAsync(entity => entity.Email == Email.Create(email), cancellationToken).ConfigureAwait(false);
        }
    }
}
