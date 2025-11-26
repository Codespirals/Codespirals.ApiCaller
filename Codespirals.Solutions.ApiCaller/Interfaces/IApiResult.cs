using System.Net;

namespace Codespirals.Solutions.ApiCaller;
public interface IApiResult<TSelf> : IResult<TSelf, string>
    where TSelf : IApiResult<TSelf>
{
    HttpStatusCode StatusCode { get; }
    static abstract TSelf Ok(HttpStatusCode statusCode);
    static abstract TSelf Fail(HttpStatusCode statusCode, string error, string? errorCode = default);
}
/// <summary>
/// A wrapper to get multiple possible results from an api call
/// </summary>
/// <typeparam name="TData"></typeparam>
public interface IApiResult<TSelf, TData> : IApiResult<TSelf>, IResultWithData<TSelf, string, TData>
    where TSelf : IApiResult<TSelf, TData>
{
    HttpStatusCode StatusCode { get; }

    static abstract TSelf Ok(HttpStatusCode statusCode, TData data);
    static abstract TSelf Fail(HttpStatusCode statusCode, string error, string? errorCode = default);
}