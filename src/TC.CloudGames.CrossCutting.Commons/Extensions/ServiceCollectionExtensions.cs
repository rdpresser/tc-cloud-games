using Microsoft.Extensions.DependencyInjection;
using TC.CloudGames.CrossCutting.Commons.Middleware;

namespace TC.CloudGames.CrossCutting.Commons.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCorrelationIdGenerator(this IServiceCollection services)
        {
            services.AddTransient<ICorrelationIdGenerator, CorrelationIdGenerator>();

            return services;
        }
    }
}
