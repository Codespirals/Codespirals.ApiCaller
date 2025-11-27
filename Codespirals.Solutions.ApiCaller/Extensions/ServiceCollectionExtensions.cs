using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Codespirals.Solutions.ApiCaller
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add an api service to your service collection
        /// </summary>
        /// <param name="serviceKey">The name of your api - your settings section must be named the same. If left empty, name the settings section "<see cref="ApiCallerService"/>".</param>
        /// <param name="configuration">The settings that contains the specific settings for this service.</param>
        public static void AddApiCaller(this IServiceCollection services, string? serviceKey = null, IConfiguration? configuration = null)
        {
            if (string.IsNullOrWhiteSpace(serviceKey))
            {
                services.AddScoped<IApiCallerService, ApiCallerService>();
                serviceKey = nameof(ApiCallerService);
            }
            else
            {
                services.AddKeyedScoped<IApiCallerService, ApiCallerService>(serviceKey);
            }
            if (configuration is not null)
            {
                IConfigurationSection configSection = configuration.GetSection(serviceKey);
                services.Configure<ApiOptions>(configSection);
            }
        }
    }
}
