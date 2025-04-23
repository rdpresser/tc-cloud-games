using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TC.CloudGames.Domain.User;

namespace TC.CloudGames.Infra.Data.Configurations
{
    internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(200)
                .HasConversion(firstName => firstName.Value, value => new FirstName(value));

            builder.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(200)
                .HasConversion(lastName => lastName.Value, value => new LastName(value));

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(200)
                .HasConversion(
                    email => email.Value.ToUpper(),
                    value => new Email(value)
                );

            builder.Property(u => u.Password)
                .IsRequired()
                .HasMaxLength(200)
                .HasConversion(password => password.Value, value => new Password(value));

            builder.Property(u => u.Role)
                .IsRequired()
                .HasMaxLength(20)
                .HasConversion(role => role.Value, value => new Role(value));

            builder.HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}
