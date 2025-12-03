using Codespirals.Base.Attributes;

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
        public required string BaseAddress { get; init; }
        public string? UserAgent { get; init; }
        /// <inheritdoc/>
        public string? Name { get; init; }
        /// <inheritdoc/>
        public Version? Version { get; init; }
        /// <inheritdoc/>
        public string? Environment { get; init; }
        /// <inheritdoc/>
        public ApiCredentials? DefaultCredentials { get; init; }
    }
}
