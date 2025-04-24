using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
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

            builder.Property(g => g.ReleaseDate);

            builder.Property(g => g.Description)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(g => g.AgeRating)
                .IsRequired()
                .HasMaxLength(10)
                .HasConversion(
                    ageRating => ageRating.Value,
                    value => AgeRating.Create(value).Value
                );

            builder.Property(g => g.Rating)
                .IsRequired()
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
                    .HasMaxLength(200);
            });

            builder.OwnsOne(g => g.GameDetails, details =>
            {
                details.Property(d => d.Platform)
                    .IsRequired()
                    .HasMaxLength(50);

                details.Property(d => d.Genre)
                    .IsRequired()
                    .HasMaxLength(50);

                details.Property(d => d.GameMode)
                    .HasMaxLength(100);

                details.Property(d => d.DistributionFormat)
                    .HasMaxLength(100);

                details.Property(d => d.AvailableLanguages)
                    .HasMaxLength(100);

                details.Property(d => d.SupportsDlcs);

                details.Property(d => d.Tags)
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
                    .IsRequired();

                playtime.Property(p => p.PlayerCount)
                    .IsRequired();
            });

            builder.Property(g => g.OfficialLink)
                .HasMaxLength(200);

            builder.Property(g => g.GameStatus)
                .HasMaxLength(200);
        }
    }
}
