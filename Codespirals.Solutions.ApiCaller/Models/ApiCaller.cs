using Codespirals.Base.Filtering;
using Codespirals.Base.Results;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Reflection;

namespace Codespirals.Solutions.ApiCaller;
public class ApiCaller
{
    private readonly ILogger<ApiCallerFactory> _logger;
    private readonly HttpClient _httpClient;
    /// <inheritdoc/>
    public string BaseUrl { get; }

    private ApiCaller(ILogger<ApiCallerFactory> logger, string baseUrl)
    {
        _logger = logger;
        BaseUrl = baseUrl;
        _httpClient = new HttpClient();
    }
    internal static ApiCaller InitiateApiCaller(ILogger<ApiCallerFactory> logger, string baseUrl)
        => new(logger, baseUrl);

    /// <inheritdoc/>
    public void SetDefaultVersion(Version? version)
    {
        if (version is null)
            return;
        _httpClient.DefaultRequestVersion = version;
    }
    /// <inheritdoc/>
    public void SetDefaultUserAgent(string? userAgent, Version? version)
    {
        userAgent ??= Assembly.GetExecutingAssembly().FullName;
        if (userAgent is null)
            return;
        _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(userAgent, version?.ToString(2)));
    }
    /// <inheritdoc/>
    public void SetDefaultApiCredentials(string keyName, string key, string? idName = null, string? id = null)
    {
        if (string.IsNullOrWhiteSpace(keyName))
            return;
        if (!string.IsNullOrWhiteSpace(idName))
            AddDefaultHeader(idName, id);
        AddDefaultHeader(keyName, key);
    }

    /// <inheritdoc/>
    public void AddDefaultHeader(string name, string? value = null)
        => _httpClient.DefaultRequestHeaders.Add(name, value);

    public HttpRequestBuilder BeginCustomApiCall()
        => HttpRequestBuilder.BeginCustomApiCall(_httpClient, _logger);

    /// <inheritdoc/>
    public async Task<ApiResult> Get(string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
        => await HttpRequestBuilder.BeginCustomApiCall(_httpClient, _logger).WithEndpoint(slug, additionalQueryParameters).Send();

    /// <inheritdoc/>
    public async Task<ApiResult<TData>> Get<TData>(string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
        => await HttpRequestBuilder.BeginCustomApiCall(_httpClient, _logger).WithEndpoint(slug, additionalQueryParameters).Send<TData>();

    /// <inheritdoc/>
    public async Task<ApiResult> Post<TBody>(TBody body, string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
        => await HttpRequestBuilder.BeginCustomApiCall(_httpClient, _logger).WithEndpoint(slug, additionalQueryParameters).WithBody(body).Send(HttpMethod.Post);

    /// <inheritdoc/>
    public async Task<ApiResult<TData>> Post<TData, TBody>(TBody body, string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
        => await HttpRequestBuilder.BeginCustomApiCall(_httpClient, _logger).WithEndpoint(slug, additionalQueryParameters).WithBody(body).Send<TData>(HttpMethod.Post);

    /// <inheritdoc/>
    public async Task<ApiResult> Put<TBody>(TBody body, string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
        => await HttpRequestBuilder.BeginCustomApiCall(_httpClient, _logger).WithEndpoint(slug, additionalQueryParameters).WithBody(body).Send(HttpMethod.Put);

    /// <inheritdoc/>
    public async Task<ApiResult<TData>> Put<TData, TBody>(TBody body, string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
        => await HttpRequestBuilder.BeginCustomApiCall(_httpClient, _logger).WithEndpoint(slug, additionalQueryParameters).WithBody(body).Send<TData>(HttpMethod.Put);

    /// <inheritdoc/>
    public async Task<ApiResult> Patch<TBody>(TBody body, string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
        => await HttpRequestBuilder.BeginCustomApiCall(_httpClient, _logger).WithEndpoint(slug, additionalQueryParameters).WithBody(body).Send(HttpMethod.Patch);

    /// <inheritdoc/>
    public async Task<ApiResult<TData>> Patch<TData, TBody>(TBody body, string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
        => await HttpRequestBuilder.BeginCustomApiCall(_httpClient, _logger).WithEndpoint(slug, additionalQueryParameters).WithBody(body).Send<TData>(HttpMethod.Patch);

    /// <inheritdoc/>
    public async Task<ApiFilteredListResult<TData, TFilterParameters>> GetPaginated<TData, TResponse, TFilterParameters>(TFilterParameters parameters, string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
        where TFilterParameters : IFilterParameters, new()
        where TResponse : IPagination<TFilterParameters>, IHasData<IEnumerable<TData>>
        => await HttpRequestBuilder.BeginCustomApiCall(_httpClient, _logger).WithEndpoint(slug, additionalQueryParameters).Search<TData, TResponse, TFilterParameters, ApiFilteredListResult<TData, TFilterParameters>>(parameters);

    /// <inheritdoc/>
    public async Task<ApiSearchResult<TData, TSearchParameters>> Search<TData, TResponse, TSearchParameters>(TSearchParameters parameters, string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
        where TSearchParameters : ISearchParameters, new()
        where TResponse : IPagination<TSearchParameters>, IHasData<IEnumerable<TData>>
        => await HttpRequestBuilder.BeginCustomApiCall(_httpClient, _logger).WithEndpoint(slug, additionalQueryParameters).Search<TData, TResponse, TSearchParameters, ApiSearchResult<TData, TSearchParameters>>(parameters);

    /// <inheritdoc/>
    public async Task<ApiResult> Delete(string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
        => await HttpRequestBuilder.BeginCustomApiCall(_httpClient, _logger).WithEndpoint(slug, additionalQueryParameters).Send(HttpMethod.Delete);

    /// <inheritdoc/>
    public async Task<ApiResult<TData>> Connect<TData>(int port, string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
        => await HttpRequestBuilder.BeginCustomApiCall(_httpClient, _logger).WithEndpoint($"{slug}:{port}", additionalQueryParameters).Send<TData>(HttpMethod.Connect);

    /// <inheritdoc/>
    public async Task<ApiResult<HttpHeaderValueCollection<string>>> Options(string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
        => await HttpRequestBuilder.BeginCustomApiCall(_httpClient, _logger).WithEndpoint(slug, additionalQueryParameters).Options();

    /// <inheritdoc/>
    public async Task<ApiResult<HttpHeaders>> Head(string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
        => await HttpRequestBuilder.BeginCustomApiCall(_httpClient, _logger).WithEndpoint(slug, additionalQueryParameters).Head();

}
