using Codespirals.Base.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace Codespirals.Solutions.ApiCaller
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add all api caller services from other services that are attributed with <see cref="RequiredConfigurationSetting"/> to your service collection
        /// </summary>
        /// <param name="configuration">The settings that contains the specific settings for the services.</param>
        public static void AddAttributedApiCallerServices(this IServiceCollection services, IConfiguration configuration)
        {
            IEnumerable<Type> apiCallerServices = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()
                .Where(t => !t.IsAbstract
                && t.GetCustomAttribute<RequiredInjectableService>() is not null
                && t.GetCustomAttribute<RequiredInjectableService>()?.Service == typeof(ApiCallerService)));

            foreach (Type? service in apiCallerServices)
            {
                if (service is null)
                    continue;
                RequiredInjectableService? attribute = service.GetCustomAttribute<RequiredInjectableService>();
                if (services.Any(s => s.ServiceType == typeof(ApiCallerService) && s.ServiceKey?.ToString() == attribute?.Key))
                    continue;

                services.TryAddAttributedService(service, attribute?.Lifetime, attribute?.Key, configuration);
            }
        }
        /// <summary>
        /// Add an api caller service 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="lifetime"></param>
        public static void AddApiCallerService(this IServiceCollection services, IConfiguration configuration, string? serviceKey = null, ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            services.Configure<ApiCallerOptions>(configuration.GetSection(serviceKey ?? nameof(ApiCallerService)));
            if (serviceKey is null)
                services.TryAdd(new ServiceDescriptor(typeof(IApiCallerService), typeof(ApiCallerService), lifetime));
            else
                services.TryAdd(new ServiceDescriptor(typeof(IApiCallerService), serviceKey, typeof(ApiCallerService), lifetime));
        }
        /// <summary>
        /// Add an api caller service 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="lifetime"></param>
        public static void AddApiCallerService(this IServiceCollection services, string baseUrl, string? serviceKey = null, ServiceLifetime lifetime = ServiceLifetime.Transient, KeyValuePair<string, string>? apiKey = null)
        {
            ApiCredentials? credentials = apiKey is not null && apiKey.HasValue ? new ApiCredentials(apiKey.Value.Key, apiKey.Value.Value) : null;
            ApiCallerOptions options = new() { BaseAddress = baseUrl, DefaultCredentials = credentials };
            services.ConfigureOptions(options);
            if (serviceKey is null)
                services.TryAdd(new ServiceDescriptor(typeof(IApiCallerService), typeof(ApiCallerService), lifetime));
            else
                services.TryAdd(new ServiceDescriptor(typeof(IApiCallerService), serviceKey, typeof(ApiCallerService), lifetime));
        }
    }
}
