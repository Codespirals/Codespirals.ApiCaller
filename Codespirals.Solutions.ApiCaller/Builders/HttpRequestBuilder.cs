using Codespirals.Base.Filtering;
using Codespirals.Base.Results;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace Codespirals.Solutions.ApiCaller;
public class HttpRequestBuilder
{
    private readonly HttpClient _httpClient;
    private readonly ILogger _logger;
    private readonly HttpRequestMessage Request;
    internal HttpRequestBuilder(HttpClient client, ILogger logger)
    {
        _httpClient = client;
        _logger = logger;
        Request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = _httpClient.BaseAddress
        };
    }

    public HttpRequestBuilder WithEndpoint(string slug, params List<KeyValuePair<string, string>> queryParameters)
    {
        slug = slug.Trim(' ', '/', '\\', '-', '_', '?');
        string parameterString = "";
        bool addAmpersand = false;
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
        Request.RequestUri = new Uri($"{_httpClient.BaseAddress}{slug}{parameterString}");
        return this;
    }
    public HttpRequestBuilder WithBody<TBody>(TBody body)
    {
        Request.Content = JsonContent.Create(body);
        return this;
    }
    public HttpRequestBuilder WithUserAgent(string agent, Version? version = null)
    {
        Request.Headers.UserAgent.Add(new ProductInfoHeaderValue(agent, version?.ToString(2)));
        return this;
    }
    public HttpRequestBuilder WithCredentials(ApiCredentials credentials)
    {
        WithHeader(credentials.Key.Name, credentials.Key.Value);
        if (credentials.Id is not null)
            WithHeader(credentials.Id.Value.Name, credentials.Id.Value.Value);
        return this;
    }
    public HttpRequestBuilder WithHeader(string name, string? value = null)
    {
        Request.Headers.Add(name, value);
        return this;
    }

    public async Task<ApiResult> Send(HttpMethod? method = null)
    {
        Request.Method = method ?? HttpMethod.Get;
        using IDisposable? log = _logger.BeginLoggingApiCall(method?.Method, Request.RequestUri?.PathAndQuery);
        using HttpResponseMessage response = await _httpClient.SendAsync(Request, HttpCompletionOption.ResponseContentRead);
        _logger.LogApiResponse(response);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogApiFail(response.ReasonPhrase);
            return ApiResult.Fail($"Api call failed.", Resources.ErrorCodes.ApiCallFailed, statusCode: response.StatusCode);
        }
        _logger.LogApiSuccess();
        return ApiResult.Ok(statusCode: response.StatusCode);
    }

    public async Task<ApiResult<TData>> Send<TData>(HttpMethod? method = null)
    {
        Request.Method = method ?? HttpMethod.Get;
        using IDisposable? log = _logger.BeginLoggingApiCall(method?.Method, Request.RequestUri?.PathAndQuery);
        using HttpResponseMessage response = await _httpClient.SendAsync(Request, HttpCompletionOption.ResponseContentRead);
        _logger.LogApiResponse(response);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogApiFail(response.ReasonPhrase);
            return ApiResult<TData>.Fail($"Api call failed.", Resources.ErrorCodes.ApiCallFailed, statusCode: response.StatusCode);
        }
        TData? content = await response.Content.ReadFromJsonAsync<TData>();
        if (content is null)
        {
            _logger.LogApiFail($"Failed to convert content to {nameof(TData)}.");
            return ApiResult<TData>.Fail($"Failed to convert content to {nameof(TData)}.", Resources.ErrorCodes.ConversionError, statusCode: response.StatusCode);
        }
        _logger.LogApiSuccess();
        return ApiResult<TData>.Ok(content, statusCode: response.StatusCode);
    }

    public async Task<TResult> Search<TData, TExpectedResponse, TFilter, TResult>(TFilter filter, HttpMethod? method = null)
        where TFilter : IFilterParameters, new()
        where TExpectedResponse : IPagination<TFilter>, IHasData<IEnumerable<TData>>
        where TResult : IApiFilteredListResult<TResult, TData, TFilter>
    {
        Request.Method = method ?? HttpMethod.Get;
        if (Request.Method == HttpMethod.Get)
            AddSearchParametersToQuery(filter);
        else if (Request.Method == HttpMethod.Post)
        {

        }
        using IDisposable? log = _logger.BeginLoggingApiCall(method?.Method, Request.RequestUri?.PathAndQuery);
        using HttpResponseMessage response = await _httpClient.SendAsync(Request, HttpCompletionOption.ResponseContentRead);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogApiFail(response.ReasonPhrase);
            return TResult.Fail(filter, "Api call failed.", Resources.ErrorCodes.ApiCallFailed, statusCode: response.StatusCode);
        }
        TExpectedResponse? content = await response.Content.ReadFromJsonAsync<TExpectedResponse>();
        if (content is null)
        {
            _logger.LogApiFail($"Failed to convert content to {nameof(TData)}.");
            return TResult.Fail(filter, "Failed .", Resources.ErrorCodes.ConversionError, statusCode: response.StatusCode);
        }
        if (content.Data is null)
        {
            _logger.LogApiFail($"The request returned no content.");
            return TResult.Fail(filter, "No content.", Resources.ErrorCodes.NoContent, statusCode: response.StatusCode);
        }
        _logger.LogApiSuccess();
        return TResult.Ok(content.Data, filter, content.TotalResults, statusCode: response.StatusCode);
    }
    public async Task<ApiResult<HttpHeaderValueCollection<string>>> Options()
    {
        Request.Method = HttpMethod.Options;
        using IDisposable? log = _logger.BeginLoggingApiCall(Request.Method.Method, Request.RequestUri?.PathAndQuery);
        var headers = await GetHeaders();
        if (headers is null)
        {
            _logger.LogApiFail($"The request returned no content.");
            return ApiResult<HttpHeaderValueCollection<string>>.Fail("No headers found.");
        }
        _logger.LogApiSuccess();
        return ApiResult<HttpHeaderValueCollection<string>>.Ok(headers.AcceptRanges);
    }
    public async Task<ApiResult<HttpHeaders>> Head()
    {
        Request.Method = HttpMethod.Head;
        using IDisposable? log = _logger.BeginLoggingApiCall(Request.Method.Method, Request.RequestUri?.PathAndQuery);
        var headers = await GetHeaders();
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
        _logger.LogApiResponse(response);
        return response.Headers;
    }
    private void AddSearchParametersToQuery<TFilter>(TFilter filter)
        where TFilter : IFilterParameters, new()
    {
        var uri = Request.RequestUri?.PathAndQuery;
        if (uri is null)
            return;
        Request.RequestUri = new Uri($"{uri}{filter.ToQueryString()}");
    }

}
