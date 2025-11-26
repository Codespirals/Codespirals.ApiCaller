using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Codespirals.Solutions.ApiCaller
{
    public static class BuilderExtensions
    {
        /// <summary>
        /// Add an api service to your service collection
        /// </summary>
        /// <param name="apiName">The name of your api - your settings section must be named the same. If left empty, name the settings section <see cref="Resources.DefaultApiName"/></param>
        public static void RegisterApiCaller(this IHostApplicationBuilder builder, string? apiName = null)
        {
            if (string.IsNullOrWhiteSpace(apiName))
            {
                builder.Services.AddScoped<IApiCallerService, ApiCallerService>();
                Microsoft.Extensions.Configuration.IConfigurationSection configSection = builder.Configuration.GetSection($"ApiSettings");
                builder.Services.Configure<ApiOptions>(configSection);
            }
            else
            {
                builder.Services.AddKeyedScoped<IApiCallerService, ApiCallerService>(apiName);
                Microsoft.Extensions.Configuration.IConfigurationSection configSection = builder.Configuration.GetSection(apiName);
                builder.Services.Configure<ApiOptions>(configSection);
            }
        }
    }
}
