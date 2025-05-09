using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Serilog;
using TC.CloudGames.Application.Exceptions;
using TC.CloudGames.Domain.Abstractions;
using TC.CloudGames.Domain.Game;
using TC.CloudGames.Domain.User;
using TC.CloudGames.Infra.Data.Configurations.Connection;
using TC.CloudGames.Infra.Data.Configurations.Data;
using TC.CloudGames.Infra.Data.Helpers;

namespace TC.CloudGames.Infra.Data
{
    public sealed class ApplicationDbContext : DbContext, IUnitOfWork, IAsyncDisposable, IDisposable
    {
        private readonly IConnectionStringProvider _connectionStringProvider;
        private bool _disposed;

        public DbSet<User> Users { get; set; }
        public DbSet<Game> Games { get; set; }

        public ApplicationDbContext(DbContextOptions options, IConnectionStringProvider connectionStringProvider)
            : base(options)
        {
            _connectionStringProvider = connectionStringProvider ?? throw new ArgumentNullException(nameof(connectionStringProvider));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                base.OnConfiguring(optionsBuilder);

                optionsBuilder.UseNpgsql(_connectionStringProvider.ConnectionString).UseSnakeCaseNamingConvention();

                // Use Serilog for EF Core logging
                optionsBuilder.LogTo(Log.Logger.Information, LogLevel.Information);

                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    optionsBuilder.EnableSensitiveDataLogging(true);
                    optionsBuilder.EnableDetailedErrors();
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            modelBuilder.HasDefaultSchema(Schemas.Default);

            // Call the method to seed users
            SeedUsers(modelBuilder);
        }

        private static void SeedUsers(ModelBuilder modelBuilder)
        {
            // Seed data for the User table with fixed GUIDs
            modelBuilder.Entity<User>().HasData(
                User.CreateWithIdForDbSeed(
                    id: Guid.Parse("11111111-1111-1111-1111-111111111111"), // Fixed GUID for Admin user
                    firstName: "Admin",
                    lastName: "User",
                    email: Email.Create("admin@admin.com").Value, // Replace with a valid email
                    password: Password.Create("Admin@123").Value, // Replace with a hashed password
                    role: Role.Create("Admin").Value,
                    createdOnUtc: new DateTime(2025, 4, 1, 0, 0, 0, DateTimeKind.Utc) // Fixed date for Admin user
                ),
                User.CreateWithIdForDbSeed(
                    id: Guid.Parse("22222222-2222-2222-2222-222222222222"), // Fixed GUID for Regular user
                    firstName: "Regular",
                    lastName: "User",
                    email: Email.Create("user@user.com").Value, // Replace with a valid email
                    password: Password.Create("User@123").Value, // Replace with a hashed password
                    role: Role.Create("User").Value,
                    createdOnUtc: new DateTime(2025, 4, 1, 0, 0, 0, DateTimeKind.Utc) // Fixed date for Regular user
                )
            );
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                await PublishDomainEventsAsync().ConfigureAwait(false);

                return result;
            }
            catch (Exception ex) when (ex.InnerException is PostgresException postEx)
            {
                throw PostgresExceptionHelper.ConvertException(postEx);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new ConcurrencyException("Concurrency exception occurred while saving changes.", ex);
            }
        }

        private async Task PublishDomainEventsAsync()
        {
            var domainEvents = ChangeTracker
                .Entries<Entity>()
                .Select(entry => entry.Entity)
                .SelectMany(entity =>
                {
                    var domainEvents = entity.GetDomainEvents();
                    entity.ClearDomainEvents();

                    return domainEvents;
                })
                .ToList();

            foreach (var domainEvent in domainEvents)
            {
                await domainEvent
                    .PublishAsync(Mode.WaitForAll).ConfigureAwait(false);
            }
        }

        public override async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();

            // Suppress finalization to prevent the finalizer from running
            GC.SuppressFinalize(this);
        }

        private async ValueTask DisposeAsyncCore()
        {
            if (!_disposed)
            {
                // Dispose any asynchronous resources here
                await base.DisposeAsync().ConfigureAwait(false);

                _disposed = true;
            }
        }

        public override void Dispose()
        {
            Dispose(true);

            // Suppress finalization to prevent the finalizer from running
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose any managed resources here
                    base.Dispose();
                }

                _disposed = true;
            }
        }
    }
}
