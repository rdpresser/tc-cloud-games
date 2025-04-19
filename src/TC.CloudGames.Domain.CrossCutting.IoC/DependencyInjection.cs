using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TC.CloudGames.Application.Users.CreateUser;

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

        private static IServiceCollection RegisterInfra(IServiceCollection services, IConfiguration configuration)
        {
            //services.AddDbContext<ApplicationDbContext>();
            //services.AddScoped<IUnitOfWork, UnitOfWork>();
            //services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

            return services;
        }

        private static IServiceCollection RegisterDomain(IServiceCollection services)
        {
            //
            return services;
        }

        private static IServiceCollection RegisterApplication(IServiceCollection services)
        {
            //services.AddScoped<Validator<CreateUserCommand>, CreateUserRequestValidator>();
            services.AddSingleton(sp => new CreateUserMapper());

            return services;
        }
    }
}
