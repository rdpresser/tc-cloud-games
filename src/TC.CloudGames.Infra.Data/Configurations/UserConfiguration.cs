using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TC.CloudGames.Domain.User;

namespace TC.CloudGames.Infra.Data.Configurations;

internal sealed class UserConfiguration : Configuration<User>
{
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
                value => Email.Create(value)
            );

        builder.Property(u => u.Password)
            .IsRequired()
            .HasMaxLength(200)
            .HasConversion(
                password => password.Value,
                value => Password.Create(value)
            );

        builder.Property(u => u.Role)
            .IsRequired()
            .HasMaxLength(20)
            .HasConversion(
                role => role.Value.ToString(),
                value => Role.Create(value)
            );

        builder.HasIndex(u => u.Email)
            .IsUnique();
    }
}