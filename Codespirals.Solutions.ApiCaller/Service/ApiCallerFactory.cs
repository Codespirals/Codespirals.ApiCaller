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

    /// <inheritdoc />
    public ApiCaller InitializeApiCaller(string baseUrl, string group = "", string? userAgent = null)
        => ApiCaller.InitiateApiCaller(_logger, baseUrl, group, userAgent);
}
