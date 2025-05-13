using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace TC.CloudGames.Infra.Data.Configurations.Data
{
    internal static class ModelBuilderExtensions
    {
        public static void ApplyConfigurationsFromAssemblyWithDI(this ModelBuilder modelBuilder, Assembly assembly, IServiceProvider serviceProvider)
        {
            var types = assembly.GetTypes()
                .Where(t => t.BaseType != null && t.BaseType.IsGenericType && t.BaseType.GetGenericTypeDefinition() == typeof(Configuration<>));

            foreach (var type in types)
            {
                var constructor = type.GetConstructors().FirstOrDefault();
                var parameters = constructor?.GetParameters().Select(p => serviceProvider.GetService(p.ParameterType)).ToArray();
                var instance = Activator.CreateInstance(type, parameters);
                modelBuilder.ApplyConfiguration(configuration: instance as dynamic);
            }
        }
    }
}
