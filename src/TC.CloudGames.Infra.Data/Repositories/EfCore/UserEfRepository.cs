using Microsoft.EntityFrameworkCore;
using TC.CloudGames.Domain.User;
using TC.CloudGames.Infra.CrossCutting.Commons.Authentication;

namespace TC.CloudGames.Infra.Data.Repositories.EfCore;

public sealed class UserEfRepository : EfRepository<User>, IUserEfRepository
{
    private readonly IPasswordHasher _passwordHasher;

    public UserEfRepository(ApplicationDbContext dbContext, IPasswordHasher passwordHasher)
        : base(dbContext)
    {
        _passwordHasher = passwordHasher;
    }

    public async Task<User?> GetByEmailWithPasswordAsync(string email, string password,
        CancellationToken cancellationToken = default)
    {
        var user = await DbContext
            .Users
            .AsNoTracking()
            .SingleOrDefaultAsync(entity =>
                entity.Email == Email.Create(email), cancellationToken).ConfigureAwait(false);

        if (user is null)
            return null;

        return _passwordHasher.Verify(password, user.Password.Value) ? user : null;
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Users
            .AsNoTracking()
            .AnyAsync(entity => entity.Email == Email.Create(email), cancellationToken).ConfigureAwait(false);
    }
}