using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using Npgsql;
using Serilog;
using System.Data;
using TC.CloudGames.Application.Exceptions;
using TC.CloudGames.Domain.Abstractions;
using TC.CloudGames.Domain.Game;
using TC.CloudGames.Domain.User;
using TC.CloudGames.Infra.Data.Configurations.Data;
using TC.CloudGames.Infra.Data.Helpers;

namespace TC.CloudGames.Infra.Data
{
    public sealed class ApplicationDbContext : DbContext, IUnitOfWork
    {
        private readonly IDatabaseConnectionProvider _connectionProvider;

        public DbSet<User> Users { get; set; }
        public DbSet<Game> Games { get; set; }

        public ApplicationDbContext(DbContextOptions options, IDatabaseConnectionProvider connectionProvider)
            : base(options)
        {
            _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                base.OnConfiguring(optionsBuilder);

                optionsBuilder.UseNpgsql(_connectionProvider.ConnectionString).UseSnakeCaseNamingConvention();

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
    }
}
