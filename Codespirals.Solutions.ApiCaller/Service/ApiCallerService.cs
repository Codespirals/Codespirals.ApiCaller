using Codespirals.Base.Attributes;
using Codespirals.Base.Filtering;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;

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
    internal readonly ILogger _logger;

    /// <inheritdoc/>
    public string BaseUrl { get; }
    /// <inheritdoc/>
    public string Name { get; }
    /// <summary>
    /// The API service to send requests to the API
    /// </summary>
    /// <param name="logger">The logger</param>
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
    internal void SetDefaultVersion(Version? version)
    {
        if (version is null)
            return;
        _httpClient.DefaultRequestVersion = version;
    }
    internal void SetDefaultUserAgent(string? userAgent, Version? version)
    {
        userAgent ??= Assembly.GetExecutingAssembly().FullName;
        if (userAgent is null)
            return;
        _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(userAgent, version?.ToString(2)));
    }
    internal void SetDefaultApiCredentials(ApiCredentials? credentials)
    {
        if (credentials is null)
            return;
        if (credentials.Id is not null)
            AddDefaultToken((KeyValuePair<string, string>)credentials.Id);
        AddDefaultToken(credentials.Key);
    }
    internal void AddDefaultToken(KeyValuePair<string, string> token)
        => _httpClient.DefaultRequestHeaders.Add(token.Key, token.Value);
    internal string BuildRequestUrl(string slug = "", params List<KeyValuePair<string, string>> queryParameters)
    {
        slug = slug.Trim(' ', '/', '\\', '-', '_', '?');
        bool addAmpersand = false;
        string parameterString = "";
        if (queryParameters.Count != 0)
        {
            StringBuilder parameterStringBuilder = new('?');
            foreach (KeyValuePair<string, string> parameter in queryParameters)
            {
                if (addAmpersand)
                    parameterStringBuilder.Append('&');
                parameterStringBuilder = parameterStringBuilder.Append(parameter.Key).Append('=').Append(parameter.Value);
                addAmpersand = true;
            }
            parameterString = parameterStringBuilder.ToString();
        }
        return $"{_httpClient.BaseAddress}{slug}{parameterString}";
    }
    /// <inheritdoc/>
    public void AddApiHeader(string name, string value)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(value))
            return;
        AddDefaultToken(new KeyValuePair<string, string>(name, value));
    }
    /// <inheritdoc/>
    public async Task<TData?> QuickGet<TData>(string slug = "", params List<KeyValuePair<string, string>> queryParameters)
    {
        using IDisposable? log = _logger.BeginLoggingApiCall(nameof(HttpMethod.Get), BaseUrl, slug);
        string endpoint = BuildRequestUrl(slug, queryParameters);
        HttpRequestMessage request = new HttpRequestBuilder(HttpMethod.Get).WithUrl(endpoint).Build();
        _logger.LogStep(LoggingExtensions.State.InProgress);
        using HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogApiFail(response.Content.IsText() ? response.Content.ToString() : null);
            return default;
        }
        _logger.LogApiSuccess();
        TData? res = await response.Content.ReadFromJsonAsync<TData>();
        return res;
    }
    /// <inheritdoc/>
    public async Task<ApiResult<TData>> Get<TData>(string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
    {
        using IDisposable? log = _logger.BeginLoggingApiCall(nameof(HttpMethod.Get), BaseUrl, slug);
        string url = BuildRequestUrl(slug, additionalQueryParameters);
        HttpRequestMessage request = new HttpRequestBuilder(HttpMethod.Get).WithUrl(url).Build();
        return await SendRequest<TData>(request);
    }

    /// <inheritdoc/>
    public async Task<ApiListResult<TData>> GetMany<TData>(string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
    {
        using IDisposable? log = _logger.BeginLoggingApiCall(nameof(HttpMethod.Get), BaseUrl, slug);
        string url = BuildRequestUrl(slug, additionalQueryParameters);
        HttpRequestMessage request = new HttpRequestBuilder(HttpMethod.Get).WithUrl(url).Build();
        return await SendRequestForMany<TData>(request);
    }

    /// <inheritdoc/>
    public async Task<ApiFilteredListResult<TData, TFilterParameters>> GetManyFiltered<TData, TFilterParameters>(TFilterParameters parameters, string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
        where TFilterParameters : IFilterParameters, new()
    {
        using IDisposable? log = _logger.BeginLoggingApiCall(nameof(HttpMethod.Get), BaseUrl, slug);
        additionalQueryParameters.AddFilterParameters(parameters);
        string url = BuildRequestUrl(slug, additionalQueryParameters);
        HttpRequestMessage request = new HttpRequestBuilder(HttpMethod.Get).WithUrl(url).Build();
        return await SendRequestForManyFiltered<TData, TFilterParameters, ApiFilteredListResult<TData, TFilterParameters>>(request, parameters);
    }

    /// <inheritdoc/>
    public async Task<ApiSearchResult<TData, TSearchParameters>> Search<TData, TSearchParameters>(TSearchParameters parameters, string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
        where TSearchParameters : ISearchParameters, new()
    {
        using IDisposable? log = _logger.BeginLoggingApiCall(nameof(HttpMethod.Get), BaseUrl, slug);
        additionalQueryParameters.AddFilterParameters(parameters);
        string url = BuildRequestUrl(slug, additionalQueryParameters);
        HttpRequestMessage request = new HttpRequestBuilder(HttpMethod.Get).WithUrl(url).Build();
        return await SendRequestForManyFiltered<TData, TSearchParameters, ApiSearchResult<TData, TSearchParameters>>(request, parameters);
    }

    /// <inheritdoc/>
    public async Task<ApiResult> Delete(string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
    {
        using IDisposable? log = _logger.BeginLoggingApiCall(nameof(HttpMethod.Delete), BaseUrl, slug);
        string url = BuildRequestUrl(slug, additionalQueryParameters);
        HttpRequestMessage request = new HttpRequestBuilder(HttpMethod.Delete).WithUrl(url).Build();
        return await SendRequestWithoutData(request);
    }

    /// <inheritdoc/>
    public async Task<ApiResult<TData>> Post<TData, TBody>(TBody body, string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
    {
        using IDisposable? log = _logger.BeginLoggingApiCall(nameof(HttpMethod.Post), BaseUrl, slug);
        string url = BuildRequestUrl(slug, additionalQueryParameters);
        HttpRequestMessage request = new HttpRequestBuilder(HttpMethod.Post).WithUrl(url).WithBody(body).Build();
        return await SendRequest<TData>(request);
    }

    /// <inheritdoc/>
    public async Task<ApiResult<TData>> Put<TData, TBody>(TBody body, string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
    {
        using IDisposable? log = _logger.BeginLoggingApiCall(nameof(HttpMethod.Put), BaseUrl, slug);
        string url = BuildRequestUrl(slug, additionalQueryParameters);
        HttpRequestMessage request = new HttpRequestBuilder(HttpMethod.Put).WithUrl(url).WithBody(body).Build();
        return await SendRequest<TData>(request);
    }

    public async Task<ApiResult<TData>> Patch<TData, TBody>(TBody body, string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
    {
        using IDisposable? log = _logger.BeginLoggingApiCall(nameof(HttpMethod.Patch), BaseUrl, slug);
        string url = BuildRequestUrl(slug, additionalQueryParameters);
        HttpRequestMessage request = new HttpRequestBuilder(HttpMethod.Patch).WithUrl(url).WithBody(body).Build();
        return await SendRequest<TData>(request);
    }

    public async Task<ApiResult<TData>> Connect<TData>(int port, string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
    {
        using IDisposable? log = _logger.BeginLoggingApiCall(nameof(HttpMethod.Get), BaseUrl, slug);
        string url = BuildRequestUrl(slug, additionalQueryParameters);
        HttpRequestMessage request = new HttpRequestBuilder(HttpMethod.Connect).WithUrl($"{url}:{port}").Build();
        return await SendRequest<TData>(request);
    }

    public async Task<HttpHeaderValueCollection<string>?> Options(string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
    {
        using IDisposable? log = _logger.BeginLoggingApiCall(nameof(HttpMethod.Get), BaseUrl, slug);
        try
        {
            string url = BuildRequestUrl(slug, additionalQueryParameters);
            HttpRequestMessage request = new HttpRequestBuilder(HttpMethod.Options).WithUrl(url).Build();
            using HttpResponseMessage response = await _httpClient.SendAsync(request) ?? throw new HttpRequestException($"No response for request {request.RequestUri}");
            response.EnsureSuccessStatusCode();
            _logger.LogApiResponseHeaders(response);
            return response.Headers.AcceptRanges;
        }
        catch (Exception e)
        {
            _logger.LogApiException(e);
            return default;
        }
    }

    public async Task<HttpHeaders?> Head(string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
    {
        using IDisposable? log = _logger.BeginLoggingApiCall(nameof(HttpMethod.Get), BaseUrl, slug);
        try
        {
            string url = BuildRequestUrl(slug, additionalQueryParameters);
            HttpRequestMessage request = new HttpRequestBuilder(HttpMethod.Head).WithUrl(url).Build();
            using HttpResponseMessage response = await _httpClient.SendAsync(request) ?? throw new HttpRequestException($"No response for request {request.RequestUri}");
            _logger.LogApiResponseHeaders(response);
            return response.Headers;
        }
        catch (Exception e)
        {
            _logger.LogApiException(e);
            return default;
        }
    }

    public async Task<ApiResult<TData>> CustomRequest<TData>(HttpRequestMessage request)
    {
        using IDisposable? log = _logger.BeginLoggingApiCall(request.Method.Method, request.RequestUri?.ToString() ?? "");
        try
        {
            return await SendRequest<TData>(request);
        }
        catch (Exception e)
        {
            _logger.LogApiException(e);
            return ApiResult<TData>.Fail(e.Message, statusCode: HttpStatusCode.BadRequest);
        }
    }

    internal async Task<ApiResult> SendRequestWithoutData(HttpRequestMessage request)
    {
        try
        {
            _logger.LogApiRequest(request);
            // make call
            using HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead);
            _logger.LogApiResponse(response);

            if (!response.IsSuccessStatusCode)
            {
                string message = await response.Content.ReadAsStringAsync();
                _logger.LogApiFail(message);
                return ApiResult.Fail("Failed to complete request.", statusCode: response.StatusCode);
            }

            _logger.LogApiSuccess();
            return ApiResult.Ok(response.StatusCode);
        }
        // unexpected other error
        catch (Exception e)
        {
            return ApiResult.Fail(e.Message);
        }
    }
    internal async Task<ApiResult<TData>> SendRequest<TData>(HttpRequestMessage request)
    {
        try
        {
            _logger.LogApiRequest(request);
            // make call
            using HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead);
            _logger.LogApiResponse(response);

            if (!response.IsSuccessStatusCode)
            {
                string message = await response.Content.ReadAsStringAsync();
                _logger.LogApiFail(message);
                return ApiResult<TData>.Fail("Failed to complete request.", statusCode: response.StatusCode);
            }

            string responseContent = await response.Content.ReadAsStringAsync();
            TData? res = await response.Content.ReadFromJsonAsync<TData>();
            if (res is null)
            {
                _logger.LogApiFail($"Error when converting response data to {typeof(TData).Name}");
                return ApiResult<TData>.Fail("No data found", statusCode: HttpStatusCode.NoContent);
            }

            _logger.LogApiSuccess();
            return ApiResult<TData>.Ok(res, statusCode: response.StatusCode);
        }
        // unexpected other error
        catch (Exception e)
        {
            return ApiResult<TData>.Fail(e.Message, statusCode: HttpStatusCode.BadRequest);
        }
    }
    /// <summary>
    /// A simple request for a list of items with no filtering of any kind
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <param name="request"></param>
    /// <returns></returns>
    internal async Task<ApiListResult<TData>> SendRequestForMany<TData>(HttpRequestMessage request)
    {
        try
        {
            _logger.LogApiRequest(request);
            // make call
            using HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead);
            _logger.LogApiResponse(response);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogApiFail($"Invalid Status code: {response.StatusCode}");
                return ApiListResult<TData>.Fail("Api call failed.", statusCode: response.StatusCode);
            }

            ICollection<TData>? content = await response.Content.ReadFromJsonAsync<ICollection<TData>>();
            if (content is null)
            {
                _logger.LogApiFail($"Error when converting response data to a list of {typeof(TData).Name}");
                return ApiListResult<TData>.Fail("Failed to convert result.", statusCode: response.StatusCode);
            }
            _logger.LogApiSuccess();
            return ApiListResult<TData>.Ok(content, statusCode: response.StatusCode);
        }
        catch (Exception e)
        {
            _logger.LogApiException(e);
            return ApiListResult<TData>.Fail($"Unknown exception triggered: {e.Message}", statusCode: HttpStatusCode.InternalServerError);
        }
    }
    /// <summary>
    /// Main API call for many items with a filter or search
    /// </summary>
    /// <typeparam name="TData">What type the items are</typeparam>
    /// <typeparam name="TFilterParameters">The type of the item filter</typeparam>
    /// <typeparam name="TResult">The type of the result</typeparam>
    /// <param name="filter"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    internal async Task<TResult> SendRequestForManyFiltered<TData, TContent, TFilterParameters, TResult>(HttpRequestMessage request, TFilterParameters filter)
        where TFilterParameters : IFilterParameters
        where TResult : IApiFilteredListResult<TResult, TData, TFilterParameters>
        where TContent : IPagination<TFilterParameters>
    {
        try
        {
            _logger.LogApiRequest(request);
            // make call
            using HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead);
            _logger.LogApiResponse(response);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogApiFail($"Invalid Status Code: {response.StatusCode}");
                return TResult.Fail(filter, "Api call failed.", statusCode: response.StatusCode);
            }
            TContent? content = await response.Content.ReadFromJsonAsync<TContent>();
            if (content is null)
            {
                _logger.LogApiFail($"Error when converting response data to a list of {typeof(TData).Name}");
                return TResult.Fail(filter, "Type error.", statusCode: response.StatusCode);
            }
            _logger.LogApiSuccess();
            // TODO Create "IHasData" interface in base
            return TResult.Ok(content.Data, content.Parameters, content.TotalResults, statusCode: response.StatusCode);
        }
        catch (Exception e)
        {
            _logger.LogApiException(e);
            return TResult.Fail(filter, e.Message, statusCode: HttpStatusCode.BadRequest);
        }
    }
}
