using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TC.CloudGames.Domain.CrossCutting.IoC
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            RegisterInfra(services, configuration);
            RegisterDomain(services);
            RegisterApplication(services);

            return services;
        }

        private static void RegisterInfra(IServiceCollection services, IConfiguration configuration)
        {
            //services.AddDbContext<ApplicationDbContext>();
            //services.AddScoped<IUnitOfWork, UnitOfWork>();
            //services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        }

        private static void RegisterDomain(IServiceCollection services)
        {
            //
        }

        private static void RegisterApplication(IServiceCollection services)
        {
            //services.AddScoped<Validator<CreateUserCommand>, CreateUserRequestValidator>();
        }
    }
}
