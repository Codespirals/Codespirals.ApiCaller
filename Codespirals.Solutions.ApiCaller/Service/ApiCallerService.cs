using Codespirals.Base.Attributes;
using Codespirals.Base.Filtering;
using Codespirals.Base.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Reflection;

namespace Codespirals.Solutions.ApiCaller;
/// <summary>
/// The implementation of the API service
/// </summary>
/// <remarks>
/// The API service to send requests to the API
/// </remarks>
/// <param name="logger">The logger</param>
[InjectableService(typeof(IApiCallerService), defaultLifetime: ServiceLifetime.Transient, optionType: typeof(ApiCallerOptions))]
[RequiredConfigurationSetting(nameof(ApiCallerOptions.BaseAddress))]
public class ApiCallerService : IApiCallerService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger _logger;

    /// <inheritdoc/>
    public string BaseUrl { get; }
    /// <inheritdoc/>
    public string Name { get; }
    /// <summary>
    /// The API service to send requests to the API
    /// </summary>
    /// <param name="logger">The logger</param>
    /// <param name="options">An option set with which to inject settings like <see cref="ApiCallerOptions.BaseAddress"/></param>
    public ApiCallerService(ILogger<ApiCallerService> logger, IOptions<ApiCallerOptions> options)
    {
        _logger = logger;
        BaseUrl = options.Value.BaseAddress;

        if (string.IsNullOrWhiteSpace(options.Value.Name))
        {
            try
            {
                string domain = RegularExpressions.MatchDomainPrefixes().Replace(BaseUrl, string.Empty);
                int index = domain.IndexOf('.');
                Name = $"{domain[0].ToString().ToUpper()}{domain.Substring(1, index).ToLowerInvariant()}-api";
            }
            catch (Exception)
            {
                Name = nameof(ApiCallerService);
            }
        }
        else
            Name = options.Value.Name;

        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(BaseUrl)
        };
        SetDefaultVersion(options.Value.Version);
        SetDefaultApiCredentials(options.Value.DefaultCredentials);
        SetDefaultUserAgent(options.Value.UserAgent, options.Value.Version);
    }
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
    public void SetDefaultApiCredentials(ApiCredentials? credentials)
    {
        if (credentials is null)
            return;
        if (credentials.Id is not null)
            AddDefaultHeader(credentials.Id.Value.Name, credentials.Id.Value.Value);
        AddDefaultHeader(credentials.Key.Name, credentials.Key.Value);
    }

    /// <inheritdoc/>
    public void AddDefaultHeader(string name, string? value = null)
        => _httpClient.DefaultRequestHeaders.Add(name, value);

    /// <inheritdoc/>
    public HttpRequestBuilder BeginCustomApiCall()
        => new(_httpClient, _logger);

    /// <inheritdoc/>
    public async Task<ApiResult> Get(string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
        => await BeginCustomApiCall().WithEndpoint(slug, additionalQueryParameters).Send();

    /// <inheritdoc/>
    public async Task<ApiResult<TData>> Get<TData>(string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
        => await BeginCustomApiCall().WithEndpoint(slug, additionalQueryParameters).Send<TData>();

    /// <inheritdoc/>
    public async Task<ApiResult> Post<TBody>(TBody body, string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
        => await BeginCustomApiCall().WithEndpoint(slug, additionalQueryParameters).WithBody(body).Send(HttpMethod.Post);

    /// <inheritdoc/>
    public async Task<ApiResult<TData>> Post<TData, TBody>(TBody body, string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
        => await BeginCustomApiCall().WithEndpoint(slug, additionalQueryParameters).WithBody(body).Send<TData>(HttpMethod.Post);

    /// <inheritdoc/>
    public async Task<ApiResult> Put<TBody>(TBody body, string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
        => await BeginCustomApiCall().WithEndpoint(slug, additionalQueryParameters).WithBody(body).Send(HttpMethod.Put);

    /// <inheritdoc/>
    public async Task<ApiResult<TData>> Put<TData, TBody>(TBody body, string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
        => await BeginCustomApiCall().WithEndpoint(slug, additionalQueryParameters).WithBody(body).Send<TData>(HttpMethod.Put);

    /// <inheritdoc/>
    public async Task<ApiResult> Patch<TBody>(TBody body, string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
        => await BeginCustomApiCall().WithEndpoint(slug, additionalQueryParameters).WithBody(body).Send(HttpMethod.Patch);

    /// <inheritdoc/>
    public async Task<ApiResult<TData>> Patch<TData, TBody>(TBody body, string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
        => await BeginCustomApiCall().WithEndpoint(slug, additionalQueryParameters).WithBody(body).Send<TData>(HttpMethod.Patch);

    /// <inheritdoc/>
    public async Task<ApiFilteredListResult<TData, TFilterParameters>> GetPaginated<TData, TResponse, TFilterParameters>(TFilterParameters parameters, string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
        where TFilterParameters : IFilterParameters, new()
        where TResponse : IPagination<TFilterParameters>, IHasData<IEnumerable<TData>>
        => await BeginCustomApiCall().WithEndpoint(slug, additionalQueryParameters).Search<TData, TResponse, TFilterParameters, ApiFilteredListResult<TData, TFilterParameters>>(parameters);

    /// <inheritdoc/>
    public async Task<ApiSearchResult<TData, TSearchParameters>> Search<TData, TResponse, TSearchParameters>(TSearchParameters parameters, string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
        where TSearchParameters : ISearchParameters, new()
        where TResponse : IPagination<TSearchParameters>, IHasData<IEnumerable<TData>>
        => await BeginCustomApiCall().WithEndpoint(slug, additionalQueryParameters).Search<TData, TResponse, TSearchParameters, ApiSearchResult<TData, TSearchParameters>>(parameters);

    /// <inheritdoc/>
    public async Task<ApiResult> Delete(string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
        => await BeginCustomApiCall().WithEndpoint(slug, additionalQueryParameters).Send(HttpMethod.Delete);

    /// <inheritdoc/>
    public async Task<ApiResult<TData>> Connect<TData>(int port, string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
        => await BeginCustomApiCall().WithEndpoint($"{slug}:{port}", additionalQueryParameters).Send<TData>(HttpMethod.Connect);

    /// <inheritdoc/>
    public async Task<ApiResult<HttpHeaderValueCollection<string>>> Options(string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
        => await BeginCustomApiCall().WithEndpoint(slug, additionalQueryParameters).Options();

    /// <inheritdoc/>
    public async Task<ApiResult<HttpHeaders>> Head(string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
        => await BeginCustomApiCall().WithEndpoint(slug, additionalQueryParameters).Head();
}
