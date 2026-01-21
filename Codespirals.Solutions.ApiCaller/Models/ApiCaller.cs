using Codespirals.Base.Filtering;
using Codespirals.Base.Results;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;

namespace Codespirals.Solutions.ApiCaller;
/// <summary>
/// A class to make API calls easier
/// </summary>
public class ApiCaller
{
    private readonly ILogger<ApiCallerFactory> _logger;
    private readonly HttpClient _httpClient;
    private string _group = "";

    private ApiCaller(ILogger<ApiCallerFactory> logger, string baseUrl, string group = "", string? userAgent = null)
    {
        _logger = logger;
        _httpClient = new HttpClient()
        {
            BaseAddress = new Uri(baseUrl)
        };
        SetGroup(group);
        SetDefaultUserAgent(userAgent);
    }
    internal static ApiCaller InitiateApiCaller(ILogger<ApiCallerFactory> logger, string baseUrl, string group = "", string? userAgent = null)
        => new(logger, baseUrl, group, userAgent);

    /// <summary>
    /// Set the default version of the <see cref="ApiCaller"/>
    /// </summary>
    /// <param name="version">The new version</param>
    public void SetDefaultVersion(Version? version)
    {
        if (version is null)
            return;
        _httpClient.DefaultRequestVersion = version;
    }

    /// <summary>
    /// Set the default user agent of the <see cref="ApiCaller"/>
    /// </summary>
    /// <param name="userAgent">The user agent</param>
    /// <param name="version">The new version</param>
    public void SetDefaultUserAgent(string? userAgent, Version? version = null)
    {
        if (userAgent is null)
            return;
        _httpClient.DefaultRequestHeaders.UserAgent.Clear();
        _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(userAgent, version?.ToString(2)));
    }

    /// <summary>
    /// Set the default api credentials of the <see cref="ApiCaller"/>
    /// </summary>
    /// <param name="keyName">The name of the key header</param>
    /// <param name="key">The actual key</param>
    /// <param name="idName">The name of the id header</param>
    /// <param name="id">The id</param>
    public void SetDefaultApiCredentials(string keyName, string key, string? idName = null, string? id = null)
    {
        if (string.IsNullOrWhiteSpace(keyName))
            return;
        if (!string.IsNullOrWhiteSpace(idName))
            AddDefaultHeader(idName, id);
        AddDefaultHeader(keyName, key);
    }

    /// <summary>
    /// Add a default header to the <see cref="ApiCaller"/>
    /// </summary>
    /// <param name="name">The name of the header</param>
    /// <param name="value">The value of the header</param>
    public void AddDefaultHeader(string name, string? value = null)
        => _httpClient.DefaultRequestHeaders.Add(name, value);

    /// <summary>
    /// Set the group of the <see cref="ApiCaller"/>
    /// </summary>
    /// <param name="group"></param>
    public void SetGroup(string group = "")
    {
        if (string.IsNullOrWhiteSpace(group))
            return;
        _group = group;
    }

    /// <summary>
    /// Start building a custom API call
    /// </summary>
    public HttpRequestBuilder BeginCustomApiCall()
        => HttpRequestBuilder.BeginCustomApiCall(_httpClient, _logger, _group);

    /// <summary>
    /// Make a <see cref="HttpMethod.Get"/> call
    /// </summary>
    /// <param name="slug">The slug to prepend to the base url</param>
    /// <param name="additionalQueryParameters">Optional additional parameters</param>
    /// <returns>An <see cref="ApiResult"/></returns>
    public async Task<ApiResult> Get(string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
        => await HttpRequestBuilder.BeginCustomApiCall(_httpClient, _logger, _group).WithEndpoint(slug, additionalQueryParameters).Send();

    /// <summary>
    /// Make a <see cref="HttpMethod.Get"/> call
    /// </summary>
    /// <typeparam name="TData">The type of the requested data</typeparam>
    /// <param name="slug">The slug to prepend to the base url</param>
    /// <param name="additionalQueryParameters">Optional additional parameters</param>
    /// <returns>An <see cref="ApiResult"/> containing the requested data</returns>
    public async Task<ApiResult<TData>> Get<TData>(string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
        => await HttpRequestBuilder.BeginCustomApiCall(_httpClient, _logger, _group).WithEndpoint(slug, additionalQueryParameters).Send<TData>();

    /// <summary>
    /// Make a <see cref="HttpMethod.Post"/> call
    /// </summary>
    /// <typeparam name="TBody"></typeparam>
    /// <param name="body"></param>
    /// <param name="slug">The slug to prepend to the base url</param>
    /// <param name="additionalQueryParameters">Optional additional parameters</param>
    /// <returns>An <see cref="ApiResult"/></returns>
    public async Task<ApiResult> Post<TBody>(string slug = "", TBody? body = default, params List<KeyValuePair<string, string>> additionalQueryParameters)
        => await HttpRequestBuilder.BeginCustomApiCall(_httpClient, _logger, _group).WithEndpoint(slug, additionalQueryParameters).WithBody(body).Send(HttpMethod.Post);

    /// <summary>
    /// Make a <see cref="HttpMethod.Post"/> call
    /// </summary>
    /// <typeparam name="TData">The type of the requested data</typeparam>
    /// <typeparam name="TBody"></typeparam>
    /// <param name="body"></param>
    /// <param name="slug">The slug to prepend to the base url</param>
    /// <param name="additionalQueryParameters">Optional additional parameters</param>
    /// <returns>An <see cref="ApiResult"/> containing the requested data</returns>
    public async Task<ApiResult<TData>> Post<TData, TBody>(string slug = "", TBody? body = default, params List<KeyValuePair<string, string>> additionalQueryParameters)
        => await HttpRequestBuilder.BeginCustomApiCall(_httpClient, _logger, _group).WithEndpoint(slug, additionalQueryParameters).WithBody(body).Send<TData>(HttpMethod.Post);

    /// <summary>
    /// Make a <see cref="HttpMethod.Put"/> call
    /// </summary>
    /// <typeparam name="TBody"></typeparam>
    /// <param name="body"></param>
    /// <param name="slug">The slug to prepend to the base url</param>
    /// <param name="additionalQueryParameters">Optional additional parameters</param>
    /// <returns>An <see cref="ApiResult"/></returns>
    public async Task<ApiResult> Put<TBody>(string slug = "", TBody? body = default, params List<KeyValuePair<string, string>> additionalQueryParameters)
        => await HttpRequestBuilder.BeginCustomApiCall(_httpClient, _logger, _group).WithEndpoint(slug, additionalQueryParameters).WithBody(body).Send(HttpMethod.Put);

    /// <summary>
    /// Make a <see cref="HttpMethod.Put"/> call
    /// </summary>
    /// <typeparam name="TData">The type of the requested data</typeparam>
    /// <typeparam name="TBody"></typeparam>
    /// <param name="body"></param>
    /// <param name="slug">The slug to prepend to the base url</param>
    /// <param name="additionalQueryParameters">Optional additional parameters</param>
    /// <returns>An <see cref="ApiResult"/> containing the requested data</returns>
    public async Task<ApiResult<TData>> Put<TData, TBody>(string slug = "", TBody? body = default, params List<KeyValuePair<string, string>> additionalQueryParameters)
        => await HttpRequestBuilder.BeginCustomApiCall(_httpClient, _logger, _group).WithEndpoint(slug, additionalQueryParameters).WithBody(body).Send<TData>(HttpMethod.Put);

    /// <summary>
    /// Make a <see cref="HttpMethod.Patch"/> call
    /// </summary>
    /// <typeparam name="TBody"></typeparam>
    /// <param name="body"></param>
    /// <param name="slug">The slug to prepend to the base url</param>
    /// <param name="additionalQueryParameters">Optional additional parameters</param>
    /// <returns>An <see cref="ApiResult"/></returns>
    public async Task<ApiResult> Patch<TBody>(string slug = "", TBody? body = default, params List<KeyValuePair<string, string>> additionalQueryParameters)
        => await HttpRequestBuilder.BeginCustomApiCall(_httpClient, _logger, _group).WithEndpoint(slug, additionalQueryParameters).WithBody(body).Send(HttpMethod.Patch);

    /// <summary>
    /// Make a <see cref="HttpMethod.Patch"/> call
    /// </summary>
    /// <typeparam name="TData">The type of the requested data</typeparam>
    /// <typeparam name="TBody"></typeparam>
    /// <param name="body"></param>
    /// <param name="slug">The slug to prepend to the base url</param>
    /// <param name="additionalQueryParameters">Optional additional parameters</param>
    /// <returns>An <see cref="ApiResult"/> containing the requested data</returns>
    public async Task<ApiResult<TData>> Patch<TData, TBody>(string slug = "", TBody? body = default, params List<KeyValuePair<string, string>> additionalQueryParameters)
        => await HttpRequestBuilder.BeginCustomApiCall(_httpClient, _logger, _group).WithEndpoint(slug, additionalQueryParameters).WithBody(body).Send<TData>(HttpMethod.Patch);

    /// <summary>
    /// Make an API call for a paginated list
    /// </summary>
    /// <typeparam name="TData">The type of the requested data</typeparam>
    /// <typeparam name="TResponse">The response type - restricted to make sure it implements <see cref="IPagination{TParamters}"/></typeparam>
    /// <typeparam name="TFilterParameters"></typeparam>
    /// <param name="parameters">The pagination parameters</param>
    /// <param name="slug">The slug to prepend to the base url</param>
    /// <param name="additionalQueryParameters">Optional additional parameters</param>
    /// <returns>An <see cref="ApiResult"/> containing the requested data</returns>
    public async Task<ApiFilteredListResult<TData, TFilterParameters>> GetPaginated<TData, TResponse, TFilterParameters>(string slug = "", TFilterParameters? parameters = default, params List<KeyValuePair<string, string>> additionalQueryParameters)
        where TFilterParameters : IFilterParameters, new()
        where TResponse : IPagination<TFilterParameters>, IHasData<IEnumerable<TData>>
        => await HttpRequestBuilder.BeginCustomApiCall(_httpClient, _logger, _group).WithEndpoint(slug, additionalQueryParameters).Search<TData, TResponse, TFilterParameters, ApiFilteredListResult<TData, TFilterParameters>>(parameters);

    /// <summary>
    /// Make an API call for a paginated list further filtered through a <see cref="ISearchParameters.Query" /> parameter
    /// </summary>
    /// <typeparam name="TData">The type of the requested data</typeparam>
    /// <typeparam name="TResponse">The response type - restricted to make sure it implements <see cref="IPagination{TParamters}"/></typeparam>
    /// <typeparam name="TSearchParameters"></typeparam>
    /// <param name="parameters">The search parameters</param>
    /// <param name="slug">The slug to prepend to the base url</param>
    /// <param name="additionalQueryParameters">Optional additional parameters</param>
    /// <returns>An <see cref="ApiResult"/> containing the requested data</returns>
    public async Task<ApiSearchResult<TData, TSearchParameters>> Search<TData, TResponse, TSearchParameters>(string slug = "", TSearchParameters? parameters = default, params List<KeyValuePair<string, string>> additionalQueryParameters)
        where TSearchParameters : ISearchParameters, new()
        where TResponse : IPagination<TSearchParameters>, IHasData<IEnumerable<TData>>
        => await HttpRequestBuilder.BeginCustomApiCall(_httpClient, _logger, _group).WithEndpoint(slug, additionalQueryParameters).Search<TData, TResponse, TSearchParameters, ApiSearchResult<TData, TSearchParameters>>(parameters);

    /// <summary>
    /// Make a <see cref="HttpMethod.Delete"/> call
    /// </summary>
    /// <param name="slug">The slug to prepend to the base url</param>
    /// <param name="additionalQueryParameters">Optional additional parameters</param>
    /// <returns>An <see cref="ApiResult"/></returns>
    public async Task<ApiResult> Delete(string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
        => await HttpRequestBuilder.BeginCustomApiCall(_httpClient, _logger, _group).WithEndpoint(slug, additionalQueryParameters).Send(HttpMethod.Delete);

    /// <summary>
    /// Make a <see cref="HttpMethod.Connect"/> call
    /// </summary>
    /// <typeparam name="TData">The type of the requested data</typeparam>
    /// <param name="port">The port to connect to</param>
    /// <param name="slug">The slug to prepend to the base url</param>
    /// <param name="additionalQueryParameters">Optional additional parameters</param>
    /// <returns>An <see cref="ApiResult"/> containing the requested data</returns>
    public async Task<ApiResult<TData>> Connect<TData>(string slug = "", int port = 0, params List<KeyValuePair<string, string>> additionalQueryParameters)
        => await HttpRequestBuilder.BeginCustomApiCall(_httpClient, _logger, _group).WithEndpoint($"{slug}:{port}", additionalQueryParameters).Send<TData>(HttpMethod.Connect);

    /// <summary>
    /// Make a <see cref="HttpMethod.Options"/> call
    /// </summary>
    /// <param name="slug">The slug to prepend to the base url</param>
    /// <param name="additionalQueryParameters">Optional additional parameters</param>
    /// <returns>An <see cref="ApiResult"/> containing the requested <see cref="HttpHeaderValueCollection{T}"/></returns>
    public async Task<ApiResult<HttpHeaderValueCollection<string>>> Options(string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
        => await HttpRequestBuilder.BeginCustomApiCall(_httpClient, _logger, _group).WithEndpoint(slug, additionalQueryParameters).Options();

    /// <summary>
    /// Make a <see cref="HttpMethod.Head"/> call
    /// </summary>
    /// <param name="slug">The slug to prepend to the base url</param>
    /// <param name="additionalQueryParameters">Optional additional parameters</param>
    /// <returns>An <see cref="ApiResult"/> containing the requested <see cref="HttpHeaders"/></returns>
    public async Task<ApiResult<HttpHeaders>> Head(string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
        => await HttpRequestBuilder.BeginCustomApiCall(_httpClient, _logger, _group).WithEndpoint(slug, additionalQueryParameters).Head();
}
