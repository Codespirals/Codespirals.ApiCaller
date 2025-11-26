using Microsoft.Extensions.Hosting;

namespace Codespirals.Solutions.ApiCaller
{
    /// <inheritdoc cref="IApiOptions"/>
    [ServiceOptions(typeof(ApiCallerService))]
    public record ApiOptions : IApiOptions
    {
        /// <inheritdoc/>
        public string Name { get; set; } = string.Empty;
        /// <inheritdoc/>
        public required string BaseAddress { get; init; }
        /// <inheritdoc/>
        public Version? Version { get; set; }
        /// <inheritdoc/>
        public ApiCredentials? DefaultCredentials { get; set; }
        /// <inheritdoc/>
        public string Environment { get; set; } = Environments.Development;
    }
}
