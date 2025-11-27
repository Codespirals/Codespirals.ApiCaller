using Codespirals.Solutions.ApiCaller.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace Codespirals.Solutions.ApiCaller;
/// <summary>
/// The implementation of the API service
/// </summary>
/// <remarks>
/// The API service to send requests to the API
/// </remarks>
/// <param name="logger">The logger</param>
[InjectableService(typeof(IApiCallerService), ServiceLifetime.Transient, isKeyed: true, optionType: typeof(ApiOptions))]
[RequiredConfigurationSetting(nameof(ApiOptions.BaseAddress))]
[RequiredInjectableService(typeof(ILogger))]
public class ApiCallerService : IApiCallerService
{
    private readonly HttpClient _httpClient;
    internal readonly ILogger _logger;

    /// <inheritdoc/>
    public string Name { get; }
    /// <inheritdoc/>
    public string BaseUrl { get; }
    /// <summary>
    /// The API service to send requests to the API
    /// </summary>
    /// <param name="logger">The logger</param>
    public ApiCallerService(ILogger<ApiCallerService> logger, IOptions<ApiOptions> options)
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
                Name = DefaultText.DefaultApiName;
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
    }
    internal void SetDefaultVersion(Version? version)
    {
        if (version is not null)
            _httpClient.DefaultRequestVersion = version;
    }
    internal void SetDefaultApiCredentials(ApiCredentials? credentials)
    {
        if (credentials is null)
            return;
        if (credentials.Id is not null)
            SetDefaultToken((KeyValuePair<string, string>)credentials.Id);
        SetDefaultToken(credentials.Key);
    }
    internal void SetDefaultToken(KeyValuePair<string, string> token)
    {
        _httpClient.DefaultRequestHeaders.Add(token.Key, token.Value);
    }
    internal string BuildRequestUrl(string slug = "", params List<KeyValuePair<string, string>> queryParameters)
    {
        slug = slug.Trim(' ', '/', '\\', '-', '_', '?');
        bool addAmpersand = false;
        var parameterString = "";
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
        SetDefaultToken(new KeyValuePair<string, string>(name, value));
    }
    /// <inheritdoc/>
    public async Task<TData?> QuickGet<TData>(string slug = "", params List<KeyValuePair<string, string>> queryParameters)
    {
        string endpoint = BuildRequestUrl(slug, queryParameters);
        HttpRequestMessage request = new HttpRequestBuilder(HttpMethod.Get).WithUrl(endpoint).Build();
        using HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead);
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
    public async Task<ApiFilteredListResult<TFilterParameters, TData>> GetManyFiltered<TData, TFilterParameters, TResult>(TFilterParameters parameters, string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
        where TFilterParameters : IFilterParameters, new()
        where TResult : IFilteredListResult<TResult, string, TFilterParameters, TData>
    {
        using IDisposable? log = _logger.BeginLoggingApiCall(nameof(HttpMethod.Get), BaseUrl, slug);
        additionalQueryParameters.AddFilterParameters(parameters);
        string url = BuildRequestUrl(slug, additionalQueryParameters);
        HttpRequestMessage request = new HttpRequestBuilder(HttpMethod.Get).WithUrl(url).Build();
        return await SendRequestForManyFiltered<TFilterParameters, TData, TResult>(parameters, request);
    }

    /// <inheritdoc/>
    public async Task<ApiFilteredListResult<TSearchParameters, TData>> Search<TData, TSearchParameters, TResult>(TSearchParameters parameters, string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
        where TSearchParameters : ISearchParameters, new()
        where TResult : IFilteredListResult<TResult, string, TSearchParameters, TData>
    {
        using IDisposable? log = _logger.BeginLoggingApiCall(nameof(HttpMethod.Get), BaseUrl, slug);
        additionalQueryParameters.AddFilterParameters(parameters);
        string url = BuildRequestUrl(slug, additionalQueryParameters);
        HttpRequestMessage request = new HttpRequestBuilder(HttpMethod.Get).WithUrl(url).Build();
        return await SendRequestForManyFiltered<TSearchParameters, TData, TResult>(parameters, request);
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
            return ApiResult<TData>.Fail(HttpStatusCode.BadRequest, e.Message);
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
                var message = await response.Content.ReadAsStringAsync();
                _logger.LogApiFail(message);
                return ApiResult.Fail(response.StatusCode, "Failed to complete request.");
            }

            _logger.LogApiSuccess();
            return ApiResult.Ok(response.StatusCode);
        }
        // unexpected other error
        catch (Exception e)
        {
            return ApiResult.Fail(HttpStatusCode.BadRequest, e.Message);
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
                var message = await response.Content.ReadAsStringAsync();
                _logger.LogApiFail(message);
                return ApiResult<TData>.Fail(response.StatusCode, "Failed to complete request.");
            }

            string responseContent = await response.Content.ReadAsStringAsync();
            TData? res = await response.Content.ReadFromJsonAsync<TData>();
            if (res is null)
            {
                _logger.LogApiFail($"Error when converting response data to {typeof(TData).Name}");
                return ApiResult<TData>.Fail(HttpStatusCode.NoContent, "No data found");
            }

            _logger.LogApiSuccess();
            return ApiResult<TData>.Ok(response.StatusCode, res);
        }
        // unexpected other error
        catch (Exception e)
        {
            return ApiResult<TData>.Fail(HttpStatusCode.BadRequest, e.Message);
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
                return ApiListResult<TData>.Fail(response.StatusCode, "Api call failed.");
            }

            ICollection<TData>? content = await response.Content.ReadFromJsonAsync<ICollection<TData>>();
            if (content is null)
            {
                _logger.LogApiFail($"Error when converting response data to a list of {typeof(TData).Name}");
                return ApiListResult<TData>.Fail(response.StatusCode, "Failed to convert result.");
            }
            _logger.LogApiSuccess();
            return ApiListResult<TData>.Ok(response.StatusCode, content);
        }
        catch (Exception e)
        {
            _logger.LogApiException(e);
            return ApiListResult<TData>.Fail(HttpStatusCode.InternalServerError, $"Unknown exception triggered: {e.Message}");
        }
    }
    /// <summary>
    /// Main API call for many items with a filter or search
    /// </summary>
    /// <typeparam name="TFilterParameters">The type of the item filter</typeparam>
    /// <typeparam name="TData">What type the items are</typeparam>
    /// <typeparam name="TResult">The type of the result</typeparam>
    /// <param name="filter"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    internal async Task<ApiFilteredListResult<TFilterParameters, TData>> SendRequestForManyFiltered<TFilterParameters, TData, TResult>(TFilterParameters filter, HttpRequestMessage request)
        where TFilterParameters : IFilterParameters, new()
        where TResult : IFilteredListResult<TResult, string, TFilterParameters, TData>
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
                return ApiFilteredListResult<TFilterParameters, TData>.Fail(filter, response.StatusCode, "Api call failed.");
            }
            TResult? content = await response.Content.ReadFromJsonAsync<TResult>();
            if (content is null)
            {
                _logger.LogApiFail($"Error when converting response data to a list of {typeof(TData).Name}");
                return ApiFilteredListResult<TFilterParameters, TData>.Fail(filter, response.StatusCode, "Type error.");
            }
            if (!content.Success || content.Data is null)
            {
                _logger.LogApiFail($"Api call successful, but the request resulted in an error: {content.Error}");
                return ApiFilteredListResult<TFilterParameters, TData>.Fail(filter, response.StatusCode, content.Error, content.ErrorCode);
            }
            _logger.LogApiSuccess();
            return ApiFilteredListResult<TFilterParameters, TData>.Ok(content.Parameters, response.StatusCode, content.Data, content.TotalResults);
        }
        catch (Exception e)
        {
            _logger.LogApiException(e);
            return ApiFilteredListResult<TFilterParameters, TData>.Fail(filter, HttpStatusCode.BadRequest, e.Message);
        }
    }
}
