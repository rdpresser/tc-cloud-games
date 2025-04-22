using Microsoft.Extensions.DependencyInjection;
using TC.CloudGames.CrossCutting.Commons.Logger;

namespace TC.CloudGames.CrossCutting.Commons.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCorrelationIdGenerator(this IServiceCollection services)
        {
            services.AddTransient<ICorrelationIdGenerator, CorrelationIdGenerator>();
            services.AddTransient(typeof(BaseLogger<>));

            return services;
        }
    }
}
