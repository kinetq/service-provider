using System.Reflection;
using Kinetq.EntityFrameworkService.Config;
using Kinetq.EntityFrameworkService.Interfaces;
using Kinetq.EntityFrameworkService.Managers;
using Kinetq.EntityFrameworkService.Resolvers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MyPericarditis.Core.Helpers;

namespace Kinetq.EntityFrameworkService.Helpers
{
    public static class ServicesConfigurationHelpers
    {
        public static void AddDataServices(this IServiceCollection services, string @namespace)
        {
            services.TryAddSingleton<ISessionManager, SessionManager>();
            services.TryAddSingleton<IConfigurationManager<EFOptions>, ConfigurationManager>();

            var moduleAssembly = Assembly.Load(new AssemblyName(@namespace));
            var persistenceConfigurations = moduleAssembly.GetTypesThatImplInterface(typeof(IPersistanceConfiguration));

            foreach (var persistenceConfiguration in persistenceConfigurations)
            {
                services.AddScoped(typeof(IPersistanceConfiguration), persistenceConfiguration);
            }
            
            var modelTypes = moduleAssembly.GetTypesInNamespace($"{@namespace}.Models").ToList();

            foreach (var modelType in modelTypes)
            {
                var idPropertyType = modelType.GetProperty("Id").PropertyType;
                var proxyType = typeof(ProxyResolver<,>).MakeGenericType(modelType, idPropertyType);
                var collectionProxyType = typeof(CollectionProxyResolver<,>).MakeGenericType(modelType, idPropertyType);

                services.AddSingleton(proxyType);
                services.AddSingleton(collectionProxyType);
            }
        }
    }
}