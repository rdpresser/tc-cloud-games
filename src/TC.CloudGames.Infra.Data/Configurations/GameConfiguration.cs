using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using TC.CloudGames.Domain.Game;

namespace TC.CloudGames.Infra.Data.Configurations
{
    internal sealed class GameConfiguration : IEntityTypeConfiguration<Game>
    {
        public void Configure(EntityTypeBuilder<Game> builder)
        {
            builder.ToTable("games");

            builder.HasKey(g => g.Id);

            builder.Property(g => g.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(g => g.ReleaseDate)
                .IsRequired();

            builder.Property(g => g.Description)
                .IsRequired(false) // Changed to IsRequired(false) as Description is nullable in the domain class
                .HasMaxLength(2000);

            builder.Property(g => g.AgeRating)
                .IsRequired()
                .HasMaxLength(10)
                .HasConversion(
                    ageRating => ageRating.Value,
                    value => AgeRating.Create(value).Value
                );

            builder.Property(g => g.Rating)
                .IsRequired(false)
                .HasConversion(
                    rating => rating.Average,
                    value => Rating.Create(value).Value
                );

            builder.OwnsOne(builder => builder.DeveloperInfo, developerInfo =>
            {
                developerInfo.Property(d => d.Developer)
                    .IsRequired()
                    .HasColumnName("developer_info")
                    .HasMaxLength(100);

                developerInfo.Property(d => d.Publisher)
                    .IsRequired(false) // Changed to IsRequired(false) as Publisher is nullable in the domain class
                    .HasMaxLength(200);
            });

            builder.OwnsOne(g => g.GameDetails, details =>
            {
                details.Property(d => d.Platform)
                    .IsRequired()
                    .HasMaxLength(2000)
                    .HasConversion(
                        v => JsonConvert.SerializeObject(v), // Assuming Platform is a list of strings
                        v => v // Assuming Platform is a list of strings
                    );

                details.Property(d => d.Genre)
                    .IsRequired(false) // Changed to IsRequired(false) as Genre is nullable in the domain class
                    .HasMaxLength(50);

                details.Property(d => d.GameMode)
                    .IsRequired()
                    .HasMaxLength(100);

                details.Property(d => d.DistributionFormat)
                    .IsRequired()
                    .HasMaxLength(100);

                details.Property(d => d.AvailableLanguages)
                    .IsRequired(false) // Changed to IsRequired(false) as AvailableLanguages is nullable in the domain class
                    .HasMaxLength(100);

                details.Property(d => d.SupportsDlcs)
                    .IsRequired();

                details.Property(d => d.Tags)
                    .IsRequired(false) // Changed to IsRequired(false) as Tags is nullable in the domain class
                    .HasMaxLength(200);
            });

            builder.OwnsOne(g => g.DiskSize, diskSize =>
            {
                diskSize.Property(d => d.SizeInGb)
                    .HasColumnName("disk_size_in_gb")
                    .IsRequired();
            });

            builder.OwnsOne(g => g.SystemRequirements, systemRequirements =>
            {
                systemRequirements.Property(s => s.Minimum)
                    .IsRequired()
                    .HasMaxLength(1000);

                systemRequirements.Property(s => s.Recommended)
                    .IsRequired(false) // Changed to IsRequired(false) as Recommended is nullable in the domain class
                    .HasMaxLength(1000);
            });

            builder.OwnsOne(g => g.Price, price =>
            {
                price.Property(p => p.Amount)
                    .IsRequired();
            });

            builder.OwnsOne(g => g.Playtime, playtime =>
            {
                playtime.Property(p => p.Hours)
                    .IsRequired(false); // Changed to IsRequired(false) as Hours is nullable in the domain class

                playtime.Property(p => p.PlayerCount)
                    .IsRequired(false); // Changed to IsRequired(false) as PlayerCount is nullable in the domain class
            });

            builder.Property(g => g.OfficialLink)
                .IsRequired(false) // Changed to IsRequired(false) as OfficialLink is nullable in the domain class
                .HasMaxLength(200);

            builder.Property(g => g.GameStatus)
                .IsRequired(false) // Changed to IsRequired(false) as GameStatus is nullable in the domain class
                .HasMaxLength(200);
        }
    }
}
