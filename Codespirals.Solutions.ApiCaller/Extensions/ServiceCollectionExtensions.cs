using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
    /// <param name="lifetime"></param>
    public static void AddApiCallerService(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        => services.TryAdd(new ServiceDescriptor(typeof(IApiCallerFactory), typeof(ApiCallerFactory), lifetime));
}
