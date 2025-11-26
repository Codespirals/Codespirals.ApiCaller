using Microsoft.Extensions.Hosting;

namespace Codespirals.Solutions.ApiCaller;
/// <summary>
/// The options necessary to set for this Api implementation to work properly
/// </summary>
public interface IApiOptions : INameable
{
    /// <summary>
    /// The base url of the API to be called
    /// </summary>
    string BaseAddress { get; }
    /// <summary>
    /// The version of the API, added to the <see cref="BaseAddress"/>
    /// </summary>
    Version? Version { get; }
    /// <summary>
    /// The user credentials that are sent with every API call
    /// </summary>
    ApiCredentials? DefaultCredentials { get; }
    /// <summary>
    /// The environement this is in. Error messages are more expressive if it's set to <see cref="Environments.Development"/>>
    /// </summary>
    string Environment { get; }
}