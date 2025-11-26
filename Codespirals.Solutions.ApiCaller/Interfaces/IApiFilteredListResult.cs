using System.Net;

namespace Codespirals.Solutions.ApiCaller;
public interface IApiFilteredListResult<TSelf, TFilter, TData> : IFilteredListResult<TSelf, string, TFilter, TData>
    where TSelf : IApiFilteredListResult<TSelf, TFilter, TData>
    where TFilter : IFilterParameters
{
    HttpStatusCode StatusCode { get; }

    static abstract TSelf Ok(TFilter filter, HttpStatusCode statusCode, IEnumerable<TData> formattedData, int totalResults);
    static abstract TSelf OkAndFormat(TFilter filter, HttpStatusCode statusCode, IEnumerable<TData> unformattedData);
    static abstract TSelf Fail(TFilter filter, HttpStatusCode statusCode, string error, string? errorCode = default);
}
