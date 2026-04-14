using Microsoft.Extensions.DependencyInjection;

namespace Codespirals.Solutions.ApiCaller;
/// <summary>
/// Extension methods for IServiceCollection to add ApiCaller services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add an api caller service 
    /// </summary>
    /// <param name="services"></param>
    public static void AddApiCallerFactory(this IServiceCollection services)
        => services.AddTransient<IApiCallerFactory, ApiCallerFactory>();
}
