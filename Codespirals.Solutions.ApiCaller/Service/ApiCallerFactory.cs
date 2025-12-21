using Codespirals.Base.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Codespirals.Solutions.ApiCaller;
/// <summary>
/// The implementation of the API service
/// </summary>
/// <param name="logger">The logger</param>
[InjectableService(typeof(IApiCallerFactory), defaultLifetime: ServiceLifetime.Scoped)]
public class ApiCallerFactory(ILogger<ApiCallerFactory> logger) : IApiCallerFactory
{
    private readonly ILogger<ApiCallerFactory> _logger = logger;

    public ApiCaller InitializeApiCaller(string baseUrl)
        => ApiCaller.InitiateApiCaller(_logger, baseUrl);
}
