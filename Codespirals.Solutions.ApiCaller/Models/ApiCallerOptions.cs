using Codespirals.Base.Attributes;
using Microsoft.Extensions.Hosting;

namespace Codespirals.Solutions.ApiCaller
{
    /// <summary>
    /// Represents configuration options for the <see cref="ApiCallerService"/>.
    /// </summary>
    /// <remarks>This class provides settings for configuring the behavior of the <see cref="ApiCallerService"/>,
    /// including the base address, API version, default credentials, and environment. 
    /// These options are typically used to initialize and customize the service.</remarks>
    [ServiceOptions(typeof(ApiCallerService))]
    public sealed class ApiCallerOptions
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
