using Codespirals.Base.Results;
using System.Net;

namespace Codespirals.Solutions.ApiCaller;
/// <summary>
/// A wrapper to get a result without data from an api call with no methods attached
/// </summary>
public interface IApiResult : IResult<string>
{
    /// <summary>
    /// The <see cref="HttpStatusCode"/> set by the API
    /// </summary>
    HttpStatusCode StatusCode { get; }
}
/// <summary>
/// A wrapper to get a result without data from an api call
/// </summary>
/// <typeparam name="TSelf">The result type itself</typeparam>
public interface IApiResult<TSelf> : IApiResult, IResult<TSelf, string>
    where TSelf : IApiResult<TSelf>
{
    /// <inheritdoc cref="IResult{TSelf, string}.Fail(string, string?)" />
    /// <param name="statusCode">An <see cref="HttpStatusCode"/></param>
    static abstract TSelf Fail(string error, string? errorCode = default, HttpStatusCode statusCode = HttpStatusCode.BadRequest);
}
/// <summary>
/// A wrapper to get a result *with* data from an api call
/// </summary>
/// <typeparam name="TData">The type of data </typeparam>
/// <typeparam name="TSelf">The result type itself</typeparam>
public interface IApiResult<TSelf, TData> : IApiResult<TSelf>, IResultWithData<TSelf, string, TData>
    where TSelf : IApiResult<TSelf, TData>
{
    /// <inheritdoc cref="IResultWithData{TSelf, TErrorCode, TData}.Ok(TData)" />
    /// <param name="statusCode">An <see cref="HttpStatusCode"/></param>
    static abstract TSelf Ok(TData data, HttpStatusCode statusCode);
}