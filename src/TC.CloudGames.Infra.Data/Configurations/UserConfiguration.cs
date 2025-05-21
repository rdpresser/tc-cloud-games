using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TC.CloudGames.Domain.User;
using TC.CloudGames.Infra.CrossCutting.Commons.Authentication;

namespace TC.CloudGames.Infra.Data.Configurations;

internal sealed class UserConfiguration : Configuration<User>
{
    private readonly IPasswordHasher _passwordHasher;

    private static Role ConvertRoleFromDatabase(string value)
    {
        return Role.Create(builder => builder.Value = value);
    }

    public UserConfiguration(IPasswordHasher passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }

    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);
        builder.ToTable("users");

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(200)
            .HasConversion(
                email => email.Value.ToUpper(),
                value => Email.CreateMap(value)
            );

        builder.Property(u => u.Password)
            .IsRequired()
            .HasMaxLength(200)
            .HasConversion(
                password => _passwordHasher.Hash(password.Value),
                value => Password.CreateMap(value)
            );

        builder.Property(u => u.Role)
            .IsRequired()
            .HasMaxLength(20)
            .HasConversion(
                role => role.ToString(),
                value => ConvertRoleFromDatabase(value)
            );

        builder.HasIndex(u => u.Email)
            .IsUnique();
    }
}