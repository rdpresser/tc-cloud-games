using Microsoft.EntityFrameworkCore;
using TC.CloudGames.Domain.User;
using TC.CloudGames.Domain.User.Abstractions;
using TC.CloudGames.Infra.CrossCutting.Commons.Authentication;
using TC.CloudGames.Infra.CrossCutting.Commons.Clock;

namespace TC.CloudGames.Infra.Data.Repositories.EfCore;

public sealed class UserEfRepository : EfRepository<User>, IUserEfRepository
{
    private readonly IPasswordHasher _passwordHasher;

    public UserEfRepository(ApplicationDbContext dbContext, IDateTimeProvider dateTimeProvider, IPasswordHasher passwordHasher)
        : base(dbContext, dateTimeProvider)
    {
        _passwordHasher = passwordHasher;
    }

    public async Task<User?> GetByEmailWithPasswordAsync(string email, string password,
        CancellationToken cancellationToken = default)
    {
        var user = await DbSet
            .AsNoTracking()
            .SingleOrDefaultAsync(entity =>
                entity.Email == Email.CreateMap(email), cancellationToken).ConfigureAwait(false);

        if (user is null)
            return null;

        return _passwordHasher.Verify(password, user.Password.Value) ? user : null;
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .AnyAsync(entity => entity.Email == Email.CreateMap(email), cancellationToken).ConfigureAwait(false);
    }
}