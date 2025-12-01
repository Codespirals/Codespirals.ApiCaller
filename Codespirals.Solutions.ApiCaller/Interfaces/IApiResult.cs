using System.Net;

namespace Codespirals.Solutions.ApiCaller;
public interface IApiResult<TSelf> : IResult<TSelf, string>
    where TSelf : IApiResult<TSelf>
{
    /// <summary>
    /// The <see cref="HttpStatusCode"/> set by the API
    /// </summary>
    HttpStatusCode StatusCode { get; }

    /// <inheritdoc cref="IResult{TSelf, string}.Fail(string, string?)" />
    /// <param name="statusCode">An <see cref="HttpStatusCode"/></param>
    static abstract TSelf Fail(string error, string? errorCode = default, HttpStatusCode statusCode = HttpStatusCode.BadRequest);
}
/// <summary>
/// A wrapper to get multiple possible results from an api call
/// </summary>
/// <typeparam name="TData"></typeparam>
public interface IApiResult<TSelf, TData> : IApiResult<TSelf>, IResultWithData<TSelf, string, TData>
    where TSelf : IApiResult<TSelf, TData>
{
    /// <inheritdoc cref="IResultWithData{TSelf, TErrorCode, TData}.Ok(TData)" />
    /// <param name="statusCode">An <see cref="HttpStatusCode"/></param>
    static abstract TSelf Ok(TData data, HttpStatusCode statusCode);
}