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
    private readonly ILogger<ApiCaller> _logger;
    private readonly HttpClient _httpClient;
    private string _group = "";

    private ApiCaller(ILogger<ApiCaller> logger, string baseUrl, string group = "", string? userAgent = null)
    {
        _logger = logger;
        _httpClient = new HttpClient()
        {
            BaseAddress = new Uri(baseUrl)
        };
        SetGroup(group);
        SetDefaultUserAgent(userAgent);
    }
    internal static ApiCaller InitiateApiCaller(ILogger<ApiCaller> logger, string baseUrl, string group = "", string? userAgent = null)
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
    /// <param name="path">The path after the group</param>
    /// <param name="slug">The slug to prepend to the base url</param>
    /// <param name="additionalQueryParameters">Optional additional parameters</param>
    /// <returns>An <see cref="ApiResult"/></returns>
    public async Task<ApiResult> Get(string path = "", string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
        => await HttpRequestBuilder.BeginCustomApiCall(_httpClient, _logger, _group).WithEndpoint(path, slug, additionalQueryParameters).Send();

    /// <summary>
    /// Make a <see cref="HttpMethod.Get"/> call
    /// </summary>
    /// <typeparam name="TData">The type of the requested data</typeparam>
    /// <param name="path">The path after the group</param>
    /// <param name="slug">The slug to prepend to the base url</param>
    /// <param name="additionalQueryParameters">Optional additional parameters</param>
    /// <returns>An <see cref="ApiResult"/> containing the requested data</returns>
    public async Task<ApiResult<TData>> Get<TData>(string path = "", string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
        => await HttpRequestBuilder.BeginCustomApiCall(_httpClient, _logger, _group).WithEndpoint(path, slug, additionalQueryParameters).Send<TData>();

    /// <summary>
    /// Make a <see cref="HttpMethod.Post"/> call
    /// </summary>
    /// <typeparam name="TBody"></typeparam>
    /// <param name="body"></param>
    /// <param name="path">The path after the group</param>
    /// <param name="slug">The slug to prepend to the base url</param>
    /// <param name="additionalQueryParameters">Optional additional parameters</param>
    /// <returns>An <see cref="ApiResult"/></returns>
    public async Task<ApiResult> Post<TBody>(string path = "", string slug = "", TBody? body = default, params List<KeyValuePair<string, string>> additionalQueryParameters)
        => await HttpRequestBuilder.BeginCustomApiCall(_httpClient, _logger, _group).WithEndpoint(path, slug, additionalQueryParameters).WithBody(body).Send(HttpMethod.Post);

    /// <summary>
    /// Make a <see cref="HttpMethod.Post"/> call
    /// </summary>
    /// <typeparam name="TData">The type of the requested data</typeparam>
    /// <typeparam name="TBody"></typeparam>
    /// <param name="body"></param>
    /// <param name="path">The path after the group</param>
    /// <param name="slug">The slug to prepend to the base url</param>
    /// <param name="additionalQueryParameters">Optional additional parameters</param>
    /// <returns>An <see cref="ApiResult"/> containing the requested data</returns>
    public async Task<ApiResult<TData>> Post<TData, TBody>(string path = "", string slug = "", TBody? body = default, params List<KeyValuePair<string, string>> additionalQueryParameters)
        => await HttpRequestBuilder.BeginCustomApiCall(_httpClient, _logger, _group).WithEndpoint(path, slug, additionalQueryParameters).WithBody(body).Send<TData>(HttpMethod.Post);

    /// <summary>
    /// Make a <see cref="HttpMethod.Put"/> call
    /// </summary>
    /// <typeparam name="TBody"></typeparam>
    /// <param name="body"></param>
    /// <param name="path">The path after the group</param>
    /// <param name="slug">The slug to prepend to the base url</param>
    /// <param name="additionalQueryParameters">Optional additional parameters</param>
    /// <returns>An <see cref="ApiResult"/></returns>
    public async Task<ApiResult> Put<TBody>(string path = "", string slug = "", TBody? body = default, params List<KeyValuePair<string, string>> additionalQueryParameters)
        => await HttpRequestBuilder.BeginCustomApiCall(_httpClient, _logger, _group).WithEndpoint(path, slug, additionalQueryParameters).WithBody(body).Send(HttpMethod.Put);

    /// <summary>
    /// Make a <see cref="HttpMethod.Put"/> call
    /// </summary>
    /// <typeparam name="TData">The type of the requested data</typeparam>
    /// <typeparam name="TBody"></typeparam>
    /// <param name="body"></param>
    /// <param name="path">The path after the group</param>
    /// <param name="slug">The slug to prepend to the base url</param>
    /// <param name="additionalQueryParameters">Optional additional parameters</param>
    /// <returns>An <see cref="ApiResult"/> containing the requested data</returns>
    public async Task<ApiResult<TData>> Put<TData, TBody>(string path = "", string slug = "", TBody? body = default, params List<KeyValuePair<string, string>> additionalQueryParameters)
        => await HttpRequestBuilder.BeginCustomApiCall(_httpClient, _logger, _group).WithEndpoint(path, slug, additionalQueryParameters).WithBody(body).Send<TData>(HttpMethod.Put);

    /// <summary>
    /// Make a <see cref="HttpMethod.Patch"/> call
    /// </summary>
    /// <typeparam name="TBody"></typeparam>
    /// <param name="body"></param>
    /// <param name="path">The path after the group</param>
    /// <param name="slug">The slug to prepend to the base url</param>
    /// <param name="additionalQueryParameters">Optional additional parameters</param>
    /// <returns>An <see cref="ApiResult"/></returns>
    public async Task<ApiResult> Patch<TBody>(string path = "", string slug = "", TBody? body = default, params List<KeyValuePair<string, string>> additionalQueryParameters)
        => await HttpRequestBuilder.BeginCustomApiCall(_httpClient, _logger, _group).WithEndpoint(path, slug, additionalQueryParameters).WithBody(body).Send(HttpMethod.Patch);

    /// <summary>
    /// Make a <see cref="HttpMethod.Patch"/> call
    /// </summary>
    /// <typeparam name="TData">The type of the requested data</typeparam>
    /// <typeparam name="TBody"></typeparam>
    /// <param name="body"></param>
    /// <param name="path">The path after the group</param>
    /// <param name="slug">The slug to prepend to the base url</param>
    /// <param name="additionalQueryParameters">Optional additional parameters</param>
    /// <returns>An <see cref="ApiResult"/> containing the requested data</returns>
    public async Task<ApiResult<TData>> Patch<TData, TBody>(string path = "", string slug = "", TBody? body = default, params List<KeyValuePair<string, string>> additionalQueryParameters)
        => await HttpRequestBuilder.BeginCustomApiCall(_httpClient, _logger, _group).WithEndpoint(path, slug, additionalQueryParameters).WithBody(body).Send<TData>(HttpMethod.Patch);

    /// <summary>
    /// Make a <see cref="HttpMethod.Query"/> call
    /// </summary>
    /// <typeparam name="TItem">The type of the requested data</typeparam>
    /// <typeparam name="TData">The data expected from the API</typeparam>
    /// <typeparam name="TBody"></typeparam>
    /// <param name="body">The query body with the requested data</param>
    /// <param name="path">The path after the group</param>
    /// <param name="slug">The slug to prepend to the base url</param>
    /// <param name="additionalQueryParameters">Optional additional parameters</param>
    /// <returns>An <see cref="ApiResult"/> containing the requested data</returns>
    public async Task<ApiResult<TData>> Query<TItem, TData, TBody>(string path = "", string slug = "", TBody? body = default, params List<KeyValuePair<string, string>> additionalQueryParameters)
        where TData : IHasData<IEnumerable<TItem>>
        => await HttpRequestBuilder.BeginCustomApiCall(_httpClient, _logger, _group).WithEndpoint(path, slug, additionalQueryParameters).WithBody(body).Send<TData>(HttpMethod.Query);

    /// <summary>
    /// Make an API call for a paginated list further filtered through a <see cref="ISearchParameters.Query" /> parameter
    /// </summary>
    /// <typeparam name="TItem">The type of the requested data</typeparam>
    /// <typeparam name="TData">The data expected from the API</typeparam>
    /// <typeparam name="TSearchParameters"></typeparam>
    /// <param name="parameters">The search parameters</param>
    /// <param name="httpMethod">The HTTP method to use (defaults to GET)</param>
    /// <param name="path">The path after the group</param>
    /// <param name="slug">The slug to prepend to the base url</param>
    /// <param name="additionalQueryParameters">Optional additional parameters</param>
    /// <returns>An <see cref="ApiResult"/> containing the requested data</returns>
    public async Task<PaginatedApiResult<TItem, TSearchParameters>> Search<TItem, TData, TSearchParameters>(string path = "", string slug = "", TSearchParameters? parameters = default, HttpMethod? httpMethod = null, params List<KeyValuePair<string, string>> additionalQueryParameters)
        where TData : IHasData<IEnumerable<TItem>>, IPagination<TSearchParameters>
        where TSearchParameters : ISearchParameters, new()
        => await HttpRequestBuilder.BeginCustomApiCall(_httpClient, _logger, _group).WithEndpoint(path, slug, additionalQueryParameters).Search<TItem, TData, TSearchParameters, PaginatedApiResult<TItem, TSearchParameters>>(parameters, httpMethod);
    
    /// <summary>
    /// Make a <see cref="HttpMethod.Query"/> call
    /// </summary>
    /// <typeparam name="TItem">The type of the requested data</typeparam>
    /// <typeparam name="TData">The data expected from the API</typeparam>
    /// <typeparam name="TSearchParameters"></typeparam>
    /// <param name="parameters">The search parameters</param>
    /// <param name="path">The path after the group</param>
    /// <param name="slug">The slug to prepend to the base url</param>
    /// <param name="additionalQueryParameters">Optional additional parameters</param>
    /// <returns>An <see cref="ApiResult"/> containing the requested data</returns>
    public async Task<PaginatedApiResult<TItem, TSearchParameters>> QueryPaginated<TItem, TData, TSearchParameters>(string path = "", string slug = "", TSearchParameters? parameters = default, params List<KeyValuePair<string, string>> additionalQueryParameters)
        where TData : IHasData<IEnumerable<TItem>>, IPagination<TSearchParameters>
        where TSearchParameters : ISearchParameters, new()
        => await HttpRequestBuilder.BeginCustomApiCall(_httpClient, _logger, _group).WithEndpoint(path, slug, additionalQueryParameters).QueryPaginated<TItem, TData, TSearchParameters, PaginatedApiResult<TItem, TSearchParameters>>(parameters);

    /// <summary>
    /// Make a <see cref="HttpMethod.Delete"/> call
    /// </summary>
    /// <param name="path">The path after the group</param>
    /// <param name="slug">The slug to prepend to the base url</param>
    /// <param name="additionalQueryParameters">Optional additional parameters</param>
    /// <returns>An <see cref="ApiResult"/></returns>
    public async Task<ApiResult> Delete(string path = "", string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
        => await HttpRequestBuilder.BeginCustomApiCall(_httpClient, _logger, _group).WithEndpoint(path, slug, additionalQueryParameters).Send(HttpMethod.Delete);

    /// <summary>
    /// Make a <see cref="HttpMethod.Connect"/> call
    /// </summary>
    /// <typeparam name="TData">The type of the requested data</typeparam>
    /// <param name="port">The port to connect to</param>
    /// <param name="path">The path after the group</param>
    /// <param name="slug">The slug to prepend to the base url</param>
    /// <param name="additionalQueryParameters">Optional additional parameters</param>
    /// <returns>An <see cref="ApiResult"/> containing the requested data</returns>
    public async Task<ApiResult<TData>> Connect<TData>(string path = "", string slug = "", int port = 0, params List<KeyValuePair<string, string>> additionalQueryParameters)
        => await HttpRequestBuilder.BeginCustomApiCall(_httpClient, _logger, _group).WithEndpoint(path, $"{slug}:{port}", additionalQueryParameters).Send<TData>(HttpMethod.Connect);

    /// <summary>
    /// Make a <see cref="HttpMethod.Options"/> call
    /// </summary>
    /// <param name="path">The path after the group</param>
    /// <param name="slug">The slug to prepend to the base url</param>
    /// <param name="additionalQueryParameters">Optional additional parameters</param>
    /// <returns>An <see cref="ApiResult"/> containing the requested <see cref="HttpHeaderValueCollection{T}"/></returns>
    public async Task<ApiResult<HttpHeaderValueCollection<string>>> Options(string path = "", string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
        => await HttpRequestBuilder.BeginCustomApiCall(_httpClient, _logger, _group).WithEndpoint(path, slug, additionalQueryParameters).Options();

    /// <summary>
    /// Make a <see cref="HttpMethod.Head"/> call
    /// </summary>
    /// <param name="path">The path after the group</param>
    /// <param name="slug">The slug to prepend to the base url</param>
    /// <param name="additionalQueryParameters">Optional additional parameters</param>
    /// <returns>An <see cref="ApiResult"/> containing the requested <see cref="HttpHeaders"/></returns>
    public async Task<ApiResult<HttpHeaders>> Head(string path = "", string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
        => await HttpRequestBuilder.BeginCustomApiCall(_httpClient, _logger, _group).WithEndpoint(path, slug, additionalQueryParameters).Head();
}
