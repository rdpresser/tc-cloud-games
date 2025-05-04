using Microsoft.EntityFrameworkCore;
using TC.CloudGames.Domain.User;
using TC.CloudGames.Infra.Data.Repositories.EfCore;

namespace TC.CloudGames.Infra.Data.Repositories;

public sealed class UserEfRepository : EfRepository<User>, IUserEfRepository
{
    public UserEfRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<User?> GetByEmailWithPasswordAsync(string email, string password,
        CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Users
            .AsNoTracking()
            .FirstOrDefaultAsync(entity =>
                entity.Email == Email.Create(email) &&
                entity.Password == Password.Create(password), cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Users
            .AsNoTracking()
            .AnyAsync(entity => entity.Email == Email.Create(email), cancellationToken).ConfigureAwait(false);
    }
}