using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Codespirals.Solutions.ApiCaller;
public class HttpRequestBuilder
{
    private HttpRequestMessage Request { get; set; }
    public HttpRequestBuilder(HttpMethod method)
    {
        Request = new HttpRequestMessage
        {
            Method = method
        };
    }
    public HttpRequestBuilder(HttpRequestMessage request)
    {
        Request = request;
    }
    public HttpRequestBuilder WithUrl(string url)
    {
        Request.RequestUri = new Uri(url);
        return new HttpRequestBuilder(Request);
    }
    public HttpRequestBuilder WithBody<TBody>(TBody body)
    {
        Request.Content = JsonContent.Create(body);
        return new HttpRequestBuilder(Request);
    }
    public HttpRequestBuilder WithUserAgent(string agent, Version? version = null)
    {
        Request.Headers.UserAgent.Add(new ProductInfoHeaderValue(agent, version?.ToString(2)));
        return new HttpRequestBuilder(Request);
    }
    public HttpRequestBuilder WithCredentials(ApiCredentials credentials)
    {
        return new HttpRequestBuilder(Request)
            .WithToken(credentials.Id)
            .WithToken(credentials.Key);
    }
    public HttpRequestBuilder WithToken(KeyValuePair<string, string>? token)
    {
        if (token is null)
            return new HttpRequestBuilder(Request);
        return WithToken(token.Value.Key, token.Value.Value);
    }
    public HttpRequestBuilder WithToken(string name, string value)
    {
        if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(value))
            Request.Headers.Add(name, value);
        return new HttpRequestBuilder(Request);
    }
    public HttpRequestMessage Build()
    {
        return Request;
    }
}
