using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TC.CloudGames.Domain.Game;

namespace TC.CloudGames.Infra.Data.Configurations
{
    internal sealed class GameConfiguration : Configuration<Game>
    {
        public override void Configure(EntityTypeBuilder<Game> builder)
        {
            base.Configure(builder);
            builder.ToTable("games");

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
                    value => AgeRating.Create(value)
                );

            builder.Property(g => g.Rating)
                .IsRequired(false)
                .HasConversion(
                    rating => rating == null || !rating.Average.HasValue ? 0 : rating.Average.Value,
                    value => Rating.Create(value)
                );

            builder.OwnsOne(x => x.DeveloperInfo, developerInfo =>
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
                    .IsRequired(false)
                    .HasConversion(
                        hours => hours ?? 0,
                        value => value
                    );

                playtime.Property(p => p.PlayerCount)
                    .IsRequired(false)
                    .HasConversion(
                        playerCount => playerCount ?? 0,
                        value => value
                    );
            });

            builder.Property(g => g.OfficialLink)
                .IsRequired(false)
                .HasMaxLength(200);

            builder.Property(g => g.GameStatus)
                .IsRequired(false)
                .HasMaxLength(200);

            builder.HasIndex(u => u.Name)
                .IsUnique(false);
        }
    }
}
