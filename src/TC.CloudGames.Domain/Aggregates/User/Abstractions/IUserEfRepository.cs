﻿namespace TC.CloudGames.Domain.Aggregates.User.Abstractions;

public interface IUserEfRepository : IEfRepository<User>
{
    Task<User?> GetByEmailWithPasswordAsync(string email, string password,
        CancellationToken cancellationToken = default);

    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
}