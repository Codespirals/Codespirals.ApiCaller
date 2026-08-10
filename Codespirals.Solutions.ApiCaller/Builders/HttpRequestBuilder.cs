using Codespirals.Base.Extensions;
using Codespirals.Base.Filtering;
using Codespirals.Base.Results;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace Codespirals.Solutions.ApiCaller;
/// <summary>
/// Builds and sends HTTP requests to an API endpoint.
/// </summary>
public class HttpRequestBuilder
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiCaller> _logger;
    private readonly HttpRequestMessage Request;

    private string _group = "";
    private string _slugWithParams = "";
    private HttpRequestBuilder(HttpClient client, ILogger<ApiCaller> logger, string group = "")
    {
        _httpClient = client;
        _logger = logger;
        Request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = _httpClient.BaseAddress
        };
        WithGroup(group);
    }
    /// <summary>
    /// Becin building a custom API call
    /// </summary>
    /// <param name="client"></param>
    /// <param name="logger"></param>
    /// <param name="group"></param>
    /// <returns></returns>
    internal static HttpRequestBuilder BeginCustomApiCall(HttpClient client, ILogger<ApiCaller> logger, string group)
        => new(client, logger, group);

    /// <summary>
    /// Sets the group for the HTTP request.
    /// </summary>
    /// <param name="group"></param>
    /// <returns></returns>
    public HttpRequestBuilder WithGroup(string group)
    {
        if (string.IsNullOrWhiteSpace(group))
            _group = "";
        else
            _group = $"{group.MakeUrlSafe('-').Trim('-')}/";
        return this;
    }

    /// <summary>
    /// Configures the HTTP request with the specified endpoint and optional query parameters.
    /// </summary>
    /// <remarks>The method constructs the full request URI by appending the trimmed <paramref name="slug"/>
    /// and the formatted query parameters to the base address of the HTTP client.</remarks>
    /// <param name="slug">The endpoint slug to append to the base address. Leading and trailing whitespace, slashes, dashes, underscores,
    /// and question marks will be trimmed.</param>
    /// <param name="queryParameters">A collection of key-value pairs representing query parameters to include in the request. If empty, no query
    /// parameters will be added.</param>
    /// <returns>The current <see cref="HttpRequestBuilder"/> instance, allowing for method chaining.</returns>
    public HttpRequestBuilder WithEndpoint(string slug, params List<KeyValuePair<string, string>> queryParameters)
    {
        slug = slug.MakeUrlSafe('-').Trim('-');
        string parameterString = "";
        if (queryParameters.Count != 0)
        {
            bool addAmpersand = false;
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
        _slugWithParams = $"{slug}{parameterString}";
        return this;
    }
    /// <summary>
    /// Sets the HTTP request body to the specified content.
    /// </summary>
    /// <remarks>The <paramref name="body"/> parameter is serialized to JSON using the default JSON
    /// serialization settings.</remarks>
    /// <typeparam name="TBody">The type of the content to be serialized as the request body.</typeparam>
    /// <param name="body">The content to include in the request body. This object will be serialized to JSON.</param>
    /// <returns>The current <see cref="HttpRequestBuilder"/> instance, allowing for method chaining.</returns>
    public HttpRequestBuilder WithBody<TBody>(TBody body)
    {
        Request.Content = JsonContent.Create(body);
        return this;
    }
    /// <summary>
    /// Sets the User-Agent header for the HTTP request.
    /// </summary>
    /// <remarks>This method clears any existing User-Agent values before adding the specified user
    /// agent.</remarks>
    /// <param name="agent">The name of the user agent to include in the header. Cannot be null or empty.</param>
    /// <param name="version">The version of the user agent, or <see langword="null"/> to omit the version.</param>
    /// <returns>The current <see cref="HttpRequestBuilder"/> instance, allowing for method chaining.</returns>
    public HttpRequestBuilder WithUserAgent(string agent, Version? version = null)
    {
        Request.Headers.UserAgent.Clear();
        Request.Headers.UserAgent.Add(new ProductInfoHeaderValue(agent, version?.ToString(2)));
        return this;
    }
    /// <summary>
    /// Adds the specified API credentials to the HTTP request as headers.
    /// </summary>
    /// <remarks>This method adds the API key as a required header and, if an ID is provided, includes it as
    /// an additional header.</remarks>
    /// <param name="keyName">The header name of the key</param>
    /// <param name="key">The value of the key</param>
    /// <param name="idName">The header name of the id (optional)</param>
    /// <param name="id">The value of the id (optional)</param>
    /// <returns>The current <see cref="HttpRequestBuilder"/> instance, allowing for method chaining.</returns>
    public HttpRequestBuilder WithCredentials(string keyName, string key, string? idName = null, string? id = null)
    {
        if (!string.IsNullOrWhiteSpace(idName))
            WithHeader(idName, id);
        WithHeader(keyName, key);
        return this;
    }
    /// <summary>
    /// Adds a header to the HTTP request being built.
    /// </summary>
    /// <param name="name">The name of the header to add. Cannot be <see langword="null"/> or empty.</param>
    /// <param name="value">The value of the header. If <see langword="null"/>, the header will be added with no value.</param>
    /// <returns>The current <see cref="HttpRequestBuilder"/> instance, allowing for method chaining.</returns>
    public HttpRequestBuilder WithHeader(string name, string? value = null)
    {
        Request.Headers.Add(name, value);
        return this;
    }

    /// <summary>
    /// Sends an HTTP request using the specified HTTP method and returns the result of the operation.
    /// </summary>
    /// <remarks>This method logs the API call, including the request details, response status, and any errors
    /// encountered.  The response is read completely before the method returns.</remarks>
    /// <param name="method">The HTTP method to use for the request. If <see langword="null"/>, the default is <see cref="HttpMethod.Get"/>.</param>
    /// <returns>An <see cref="ApiResult"/> representing the outcome of the API call.  Returns a successful result if the HTTP
    /// response indicates success; otherwise, returns a failure result with the appropriate status code and error
    /// message.</returns>
    public async Task<ApiResult> Send(HttpMethod? method = null)
    {
        Request.RequestUri = new Uri($"{_httpClient.BaseAddress}{_group}{_slugWithParams}");
        Request.Method = method ?? HttpMethod.Get;
        using IDisposable? log = _logger.BeginLoggingApiCall(method?.Method, Request.RequestUri?.PathAndQuery);
        using HttpResponseMessage response = await _httpClient.SendAsync(Request, HttpCompletionOption.ResponseContentRead);
        await _logger.LogApiResponse(response);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogApiFail(response.ReasonPhrase);
            return ApiResult.Fail($"Api call failed.", ApiCallerConstants.ErrorCodeApiCallFailed, statusCode: response.StatusCode);
        }
        _logger.LogApiSuccess();
        return ApiResult.Ok(statusCode: response.StatusCode);
    }

    /// <summary>
    /// Sends an HTTP request using the specified HTTP method and processes the response.
    /// </summary>
    /// <typeparam name="TData">The type of the data expected in the response content.</typeparam>
    /// <param name="method">The HTTP method to use for the request. Defaults to <see cref="HttpMethod.Get"/> if not specified.</param>
    /// <returns>An <see cref="ApiResult{TData}"/> containing the result of the API call. If the request is successful, the
    /// result contains the deserialized response content of type <typeparamref name="TData"/>. If the request fails or
    /// the content cannot be deserialized, the result contains an error message and status code.</returns>
    public async Task<ApiResult<TData>> Send<TData>(HttpMethod? method = null)
    {
        Request.RequestUri = new Uri($"{_httpClient.BaseAddress}{_group}{_slugWithParams}");
        Request.Method = method ?? HttpMethod.Get;
        using IDisposable? log = _logger.BeginLoggingApiCall(method?.Method, Request.RequestUri?.PathAndQuery);
        using HttpResponseMessage response = await _httpClient.SendAsync(Request, HttpCompletionOption.ResponseContentRead);
        await _logger.LogApiResponse(response);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogApiFail(response.ReasonPhrase);
            return ApiResult<TData>.Fail($"Api call failed.", ApiCallerConstants.ErrorCodeApiCallFailed, statusCode: response.StatusCode);
        }
        TData? content = await response.Content.ReadFromJsonAsync<TData>();
        if (content is null)
        {
            _logger.LogApiFail($"Failed to convert content to {nameof(TData)}.");
            return ApiResult<TData>.Fail($"Failed to convert content to {nameof(TData)}.", ApiCallerConstants.ErrorCodeConversionError, statusCode: response.StatusCode);
        }
        _logger.LogApiSuccess();
        return ApiResult<TData>.Ok(content, statusCode: response.StatusCode);
    }
    /// <summary>
    /// Executes a search operation using the specified filter and HTTP method
    /// </summary>
    /// <typeparam name="TItem">The type of the data items returned by the search.</typeparam>
    /// <typeparam name="TData">The entire requested data, with items and everything needed for pagination</typeparam>
    /// <typeparam name="TSearchParameters">The type of the filter parameters used to refine the search, which must implement <see
    /// cref="IFilterParameters"/> and have a parameterless constructor.</typeparam>
    /// <typeparam name="TResult">The type of the result returned by the search, which must implement <see cref="IPaginatedApiResult{TResult,
    /// TData, TFilter}"/>.</typeparam>
    /// <param name="filter">The filter parameters used to refine the search. This determines the criteria for the search operation.</param>
    /// <param name="method">The HTTP method to use for the request. Defaults to <see cref="HttpMethod.Get"/> if not specified.</param>
    /// <returns>An instance of <typeparamref name="TResult"/> containing the search results, including the data items, filter
    /// parameters, and total result count. If the operation fails, the result will indicate the failure reason and
    /// status code.</returns>
    public async Task<TResult> Search<TItem, TData, TSearchParameters, TResult>(TSearchParameters? filter, HttpMethod? method = null)
        where TData : IHasData<IEnumerable<TItem>>, IPagination<TSearchParameters>
        where TSearchParameters : IFilterParameters, new()
        where TResult : IPaginatedApiResult<TResult, TItem, TSearchParameters>
    {
        Request.RequestUri = new Uri($"{_httpClient.BaseAddress}{_group}{_slugWithParams}");
        Request.Method = method ?? HttpMethod.Get;
        filter ??= new TSearchParameters();

        if (Request.Method == HttpMethod.Post || Request.Method == HttpMethod.Put)
            Request.Content = JsonContent.Create(filter);
        else
        {
            Request.Method = HttpMethod.Get;
            AddSearchParametersToQuery(filter);
        }

        using IDisposable? log = _logger.BeginLoggingApiCall(method?.Method, Request.RequestUri?.PathAndQuery);
        using HttpResponseMessage response = await _httpClient.SendAsync(Request, HttpCompletionOption.ResponseContentRead);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogApiFail(response.ReasonPhrase);
            return TResult.Fail(filter, "Api call failed.", ApiCallerConstants.ErrorCodeApiCallFailed, statusCode: response.StatusCode);
        }
        TData? data = await response.Content.ReadFromJsonAsync<TData>();
        if (data is null)
        {
            _logger.LogApiFail($"Failed to convert content to {nameof(TItem)}.");
            return TResult.Fail(filter, "Failed .", ApiCallerConstants.ErrorCodeConversionError, statusCode: response.StatusCode);
        }
        if (data.Data is null)
        {
            _logger.LogApiFail($"The request returned no content.");
            return TResult.Fail(filter, "No content.", ApiCallerConstants.ErrorCodeNoContent, statusCode: response.StatusCode);
        }
        _logger.LogApiSuccess();
        return TResult.Ok(data.Data, data.Parameters, data.TotalResults, statusCode: response.StatusCode);
    }
    /// <summary>
    /// Sends an HTTP OPTIONS request to the configured endpoint and retrieves the "Accept-Ranges" headers from the
    /// response.
    /// </summary>
    /// <remarks>This method performs an asynchronous HTTP OPTIONS request using the current configuration of
    /// the request.  If the response contains "Accept-Ranges" headers, they are returned as part of the result.  If no
    /// headers are found, the method returns a failure result.</remarks>
    /// <returns>An <see cref="ApiResult{T}"/> containing a collection of "Accept-Ranges" header values if present.  Returns a
    /// failure result if the response does not include any headers.</returns>
    public async Task<ApiResult<HttpHeaderValueCollection<string>>> Options()
    {
        Request.RequestUri = new Uri($"{_httpClient.BaseAddress}{_group}{_slugWithParams}");
        Request.Method = HttpMethod.Options;
        using IDisposable? log = _logger.BeginLoggingApiCall(Request.Method.Method, Request.RequestUri?.PathAndQuery);
        HttpResponseHeaders? headers = await GetHeaders();
        if (headers is null)
        {
            _logger.LogApiFail($"The request returned no content.");
            return ApiResult<HttpHeaderValueCollection<string>>.Fail("No headers found.");
        }
        _logger.LogApiSuccess();
        return ApiResult<HttpHeaderValueCollection<string>>.Ok(headers.AcceptRanges);
    }
    /// <summary>
    /// Sends an HTTP HEAD request and retrieves the response headers.
    /// </summary>
    /// <remarks>This method sends an HTTP HEAD request to the configured URI and returns the headers from the
    /// response. If the response does not contain any headers, the method returns a failure result.</remarks>
    /// <returns>An <see cref="ApiResult{T}"/> containing the response headers if the request is successful;  otherwise, a
    /// failure result with an appropriate error message.</returns>
    public async Task<ApiResult<HttpHeaders>> Head()
    {
        Request.RequestUri = new Uri($"{_httpClient.BaseAddress}{_group}{_slugWithParams}");
        Request.Method = HttpMethod.Head;
        using IDisposable? log = _logger.BeginLoggingApiCall(Request.Method.Method, Request.RequestUri?.PathAndQuery);
        HttpResponseHeaders? headers = await GetHeaders();
        if (headers is null)
        {
            _logger.LogApiFail($"The request returned no content.");
            return ApiResult<HttpHeaders>.Fail("No headers ");
        }
        _logger.LogApiSuccess();
        return ApiResult<HttpHeaders>.Ok(headers);
    }
    private async Task<HttpResponseHeaders?> GetHeaders()
    {
        using HttpResponseMessage response = await _httpClient.SendAsync(Request, HttpCompletionOption.ResponseContentRead);
        await _logger.LogApiResponse(response);
        return response.Headers;
    }
    private void AddSearchParametersToQuery<TFilter>(TFilter filter)
        where TFilter : IFilterParameters, new()
    {
        string? uri = Request.RequestUri?.PathAndQuery;
        if (uri is null)
            return;
        Request.RequestUri = new Uri($"{uri}{filter.ToQueryString()}");
    }
}
