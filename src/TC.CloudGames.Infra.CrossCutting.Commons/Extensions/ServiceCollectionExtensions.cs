using Microsoft.Extensions.DependencyInjection;
using TC.CloudGames.Infra.CrossCutting.Commons.Middleware;

namespace TC.CloudGames.Infra.CrossCutting.Commons.Extensions
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
