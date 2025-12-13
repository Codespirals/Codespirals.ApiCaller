using Codespirals.Base.Filtering;
using Codespirals.Base.Results;
using System.Net.Http.Headers;

namespace Codespirals.Solutions.ApiCaller;
/// <summary>
/// The interface depicting the main service that interacts with the Collabour API
/// </summary>
public interface IApiCallerService : INameable
{
    /// <summary>
    /// The base url of the API being called
    /// </summary>
    public string BaseUrl { get; }

    /// <summary>
    /// Sets the default version to be used by the application.
    /// </summary>
    /// <remarks>This method updates the default version used for operations that depend on versioning. 
    /// Passing <see langword="null"/> reverts the application to using no default version.</remarks>
    /// <param name="version">The version to set as the default. If <see langword="null"/>, the default version will be cleared.</param>
    public void SetDefaultVersion(Version? version);
    /// <summary>
    /// Sets the default user agent string and version for outgoing requests.
    /// </summary>
    /// <remarks>This method allows customization of the user agent string and version used in outgoing
    /// requests. If both parameters are <see langword="null"/> no user agent will be set.</remarks>
    /// <param name="userAgent">The user agent string to use.</param>
    /// <param name="version">The version associated with the user agent. Can be <see langword="null"/> if no version is specified.</param>
    public void SetDefaultUserAgent(string? userAgent, Version? version);
    /// <summary>
    /// Sets the default API credentials to be used for authentication in subsequent API requests.
    /// </summary>
    /// <remarks>This method updates the default credentials used by the application. If no credentials are
    /// set,  API requests may fail if authentication is required.</remarks>
    /// <param name="credentials">The <see cref="ApiCredentials"/> instance containing the API key and secret.  Pass <see langword="null"/> to
    /// clear the default credentials.</param>
    public void SetDefaultApiCredentials(ApiCredentials? credentials);
    /// <summary>
    /// Add a default header sent with every api call
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public void AddDefaultHeader(string name, string value);

    /// <summary>
    /// Begins the construction of a custom API call by returning an instance of <see cref="HttpRequestBuilder"/>.
    /// </summary>
    /// <remarks>Use the returned <see cref="HttpRequestBuilder"/> to configure the HTTP request, including
    /// setting headers, query parameters, and the request body. Once configured, the request can be executed to
    /// interact with the target API.</remarks>
    /// <returns>An instance of <see cref="HttpRequestBuilder"/> that allows customization of the HTTP request.</returns>
    public HttpRequestBuilder BeginCustomApiCall();

    /// <summary>
    /// Send a create request to the API
    /// </summary>
    /// <typeparam name="TBody">The type of the data being sent</typeparam>
    /// <param name="body">The body data</param>
    /// <param name="slug">A slug string (optional)</param>
    /// <returns>A result pattern item containing the requested item, or an explanation of why it failed.</returns>
    public Task<ApiResult> Post<TBody>(TBody body, string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters);

    /// <inheritdoc cref="Post{TBody}(TBody, string, List{KeyValuePair{string, string}})"/>
    /// <typeparam name="TData">The expected return type</typeparam>
    public Task<ApiResult<TData>> Post<TData, TBody>(TBody body, string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters);

    /// <summary>
    /// Send an update request to the API
    /// </summary>
    /// <typeparam name="TBody">The type of the data being sent</typeparam>
    /// <param name="body">The body data</param>
    /// <param name="slug">A slug string (optional)</param>
    /// <returns>A result pattern item containing the requested item, or an explanation of why it failed.</returns>
    public Task<ApiResult> Put<TBody>(TBody body, string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters);
    
    /// <inheritdoc cref="Put{TBody}(TBody, string, List{KeyValuePair{string, string}})"/>
    /// <typeparam name="TData">The expected return type</typeparam>
    public Task<ApiResult<TData>> Put<TData, TBody>(TBody body, string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters);

    /// <summary>
    /// Send a patch request to the API
    /// Patch requests are generally used when an item is only partially replaced
    /// </summary>
    /// <typeparam name="TBody">The type of the data being sent</typeparam>
    /// <param name="body">The body data</param>
    /// <param name="slug">A slug string (optional)</param>
    /// <returns>A result pattern item containing the requested item, or an explanation of why it failed.</returns>
    public Task<ApiResult> Patch<TBody>(TBody body, string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters);

    /// <inheritdoc cref="Patch{TBody}(TBody, string, List{KeyValuePair{string, string}})"/>
    /// <typeparam name="TData">The expected return type</typeparam>
    public Task<ApiResult<TData>> Patch<TData, TBody>(TBody body, string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters);
    
    /// <summary>
    /// Send a request to get a resource to the API
    /// </summary>
    /// <param name="slug">A slug string (optional)</param>
    /// <returns>A result pattern item containing the requested item, or an explanation of why it failed.</returns>
    public Task<ApiResult> Get(string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters);

    /// <inheritdoc cref="Get(string, List{KeyValuePair{string, string}})"/>
    /// <typeparam name="TData">The expected return type</typeparam>
    public Task<ApiResult<TData>> Get<TData>(string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters);

    /// <summary>
    /// Send a GET request to get a partial list of resources to the API
    /// </summary>
    /// <typeparam name="TData">The expected return type</typeparam>
    /// <param name="slug">A slug string (optional)</param>
    /// <returns>An api result with the requested section of the list as well as the total possible results found.</returns>
    public Task<ApiFilteredListResult<TData, TFilterParameters>> GetPaginated<TData, TResponse, TFilterParameters>(TFilterParameters paramters, string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
        where TFilterParameters : IFilterParameters, new()
        where TResponse : IPagination<TFilterParameters>, IHasData<IEnumerable<TData>>;

    /// <summary>
    /// Send a GET request to get a partial list of resources to the API
    /// </summary>
    /// <typeparam name="TData">The expected return type</typeparam>
    /// <param name="slug">A slug string (optional)</param>
    /// <returns>An api result with the requested section of the list as well as the total possible results found.</returns>
    public Task<ApiSearchResult<TData, TSearchParameters>> Search<TData, TResponse, TSearchParameters>(TSearchParameters paramters, string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
        where TSearchParameters : ISearchParameters, new()
        where TResponse : IPagination<TSearchParameters>, IHasData<IEnumerable<TData>>;

    /// <summary>
    /// Send a delete request to the API
    /// </summary>
    /// <param name="slug">A slug string (optional)</param>
    /// <returns>A result pattern item containing the requested item, or an explanation of why it failed.</returns>
    public Task<ApiResult> Delete(string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters);

    /// <summary>
    /// Send a request create a tunnel to the API
    /// </summary>
    /// <typeparam name="TData">The expected return type</typeparam>
    /// <param name="port">The port that should be connected to</param>
    /// <param name="slug">A slug string (optional)</param>
    /// <returns>A result pattern item containing the requested item, or an explanation of why it failed.</returns>
    public Task<ApiResult<TData>> Connect<TData>(int port, string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters);

    /// <summary>
    /// Sends an HTTP HEAD request to the specified resource and retrieves the response headers.
    /// </summary>
    /// <remarks>Use this method to check the metadata of a resource without downloading its content. The
    /// method allows adding custom query parameters to the request.</remarks>
    /// <param name="slug">An optional path segment appended to the base URL. If not provided, the base URL is used.</param>
    /// <param name="additionalQueryParameters">A collection of key-value pairs representing additional query parameters to include in the request.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="ApiResult{T}"/> object
    /// with the HTTP response headers as <see cref="HttpHeaders"/>.</returns>
    public Task<ApiResult<HttpHeaders>> Head(string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters);

    /// <summary>
    /// Sends an HTTP OPTIONS request to the specified resource and retrieves the response headers.
    /// </summary>
    /// <remarks>Use this method to determine the communication options available for a specific resource. The
    /// response typically includes metadata about the resource, such as allowed HTTP methods.</remarks>
    /// <param name="slug">An optional resource identifier appended to the base URL. Defaults to an empty string.</param>
    /// <param name="additionalQueryParameters">A collection of key-value pairs representing additional query parameters to include in the request.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="ApiResult{T}"/> object
    /// with a collection of HTTP header values returned by the server.</returns>
    public Task<ApiResult<HttpHeaderValueCollection<string>>> Options(string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters);
}