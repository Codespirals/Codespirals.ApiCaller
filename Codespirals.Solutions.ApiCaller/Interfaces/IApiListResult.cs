using System.Net;

namespace Codespirals.Solutions.ApiCaller;
public interface IApiListResult<TSelf, TData> : IListResult<TSelf, string, TData>
    where TSelf : IApiListResult<TSelf, TData>
{
    HttpStatusCode StatusCode { get; }

    static abstract TSelf Ok(HttpStatusCode statusCode, IEnumerable<TData> formattedData);
    static abstract TSelf Fail(HttpStatusCode statusCode, string error, string? errorCode = default);
}
