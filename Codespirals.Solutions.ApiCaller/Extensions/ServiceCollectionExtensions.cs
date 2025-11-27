using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Codespirals.Solutions.ApiCaller
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add an api service to your service collection
        /// </summary>
        /// <param name="serviceKey">The name of your api - your settings section must be named the same. If left empty, name the settings section "<see cref="ApiCallerService"/>".</param>
        /// <param name="configuration">The settings that contains the specific settings for this service.</param>
        public static void AddApiCallerServices(this IServiceCollection services, IConfiguration? configuration = null)
        {
            // TODO: Determine if config is needed or only env variable will do
            var apiCallerServices = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()
                .Where(t => !t.IsAbstract
                && t.GetCustomAttribute<RequiredInjectableService>() is not null
                && t.GetCustomAttribute<RequiredInjectableService>()?.Service == typeof(ApiCallerService)));

            foreach (var service in apiCallerServices)
            {
                var attribute = service.GetCustomAttribute<RequiredInjectableService>();
                if (services.Any(s => s.ServiceType == typeof(ApiCallerService) && s.ServiceKey?.ToString() == attribute?.Key))
                    continue;

                if (string.IsNullOrWhiteSpace(attribute?.Key))
                    services.AddScoped<IApiCallerService, ApiCallerService>();
                else
                {
                    services.AddKeyedScoped<IApiCallerService, ApiCallerService>(attribute?.Key);
                }
                if (configuration is not null)
                {
                    IConfigurationSection configSection = configuration.GetSection(attribute?.Key ?? nameof(ApiCallerService));
                    services.Configure<ApiOptions>(configSection);
                }
            }
        }
    }
}
