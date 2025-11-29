using Microsoft.Extensions.Hosting;

namespace Codespirals.Solutions.ApiCaller
{
    /// <inheritdoc cref="IApiCallerOptions"/>
    [ServiceOptions(typeof(ApiCallerService))]
    public sealed class ApiCallerOptions : IApiCallerOptions
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
