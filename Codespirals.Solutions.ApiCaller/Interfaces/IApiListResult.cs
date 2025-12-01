using System.Net;

namespace Codespirals.Solutions.ApiCaller;
public interface IApiListResult<TSelf, TData> : IListResult<TSelf, string, TData>
    where TSelf : IApiListResult<TSelf, TData>
{
    HttpStatusCode StatusCode { get; }

    static abstract TSelf Ok(IEnumerable<TData> formattedData, HttpStatusCode statusCode);
    static abstract TSelf Fail(string error, string? errorCode = default, HttpStatusCode statusCode = HttpStatusCode.BadRequest);
}
