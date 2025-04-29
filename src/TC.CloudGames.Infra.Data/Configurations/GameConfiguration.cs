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

            builder.Property(g => g.ReleaseDate)
                .IsRequired();

            builder.Property(g => g.Description)
                .IsRequired(false)
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
                    .IsRequired(false)
                    .HasMaxLength(200);
            });

            builder.OwnsOne(g => g.GameDetails, details =>
            {
                details.Property(d => d.Platform)
                    .IsRequired()
                    .HasMaxLength(2000);

                details.Property(d => d.Genre)
                    .IsRequired(false)
                    .HasMaxLength(50);

                details.Property(d => d.GameMode)
                    .IsRequired()
                    .HasMaxLength(100);

                details.Property(d => d.DistributionFormat)
                    .IsRequired()
                    .HasMaxLength(100);

                details.Property(d => d.AvailableLanguages)
                    .IsRequired(false)
                    .HasMaxLength(100);

                details.Property(d => d.SupportsDlcs)
                    .IsRequired();

                details.Property(d => d.Tags)
                    .IsRequired(false)
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
                    .IsRequired(false)
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
                    .IsRequired(false);

                playtime.Property(p => p.PlayerCount)
                    .IsRequired(false);
            });

            builder.Property(g => g.OfficialLink)
                .IsRequired(false)
                .HasMaxLength(200);

            builder.Property(g => g.GameStatus)
                .IsRequired(false)
                .HasMaxLength(200);
        }
    }
}
