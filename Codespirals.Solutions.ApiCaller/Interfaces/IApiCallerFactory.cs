namespace Codespirals.Solutions.ApiCaller;
/// <summary>
/// The interface depicting the main service that interacts with the Collabour API
/// </summary>
public interface IApiCallerFactory
{
    /// <summary>
    /// Create a new Api Caller with the specific base URL
    /// </summary>
    /// <param name="baseUrl">The url of the API</param>
    /// <param name="userAgent">An optional user agent to tell the API who made the calls</param>
    /// <returns>The Api Caller</returns>
    public ApiCaller InitializeApiCaller(string baseUrl, string? userAgent = null);
}