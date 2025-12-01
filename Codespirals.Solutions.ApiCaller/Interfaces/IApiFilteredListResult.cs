using System.Net;

namespace Codespirals.Solutions.ApiCaller;
public interface IApiFilteredListResult<TSelf, TData, TFilter> : IApiResult<TSelf>, IFilteredListResult<TSelf, string, TData, TFilter>
    where TSelf : IApiFilteredListResult<TSelf, TData, TFilter>
    where TFilter : IFilterParameters
{
    /// <inheritdoc />
    static abstract TSelf Ok(IEnumerable<TData> formattedData, TFilter filter, int totalResults, HttpStatusCode statusCode);
    /// <inheritdoc />
    static abstract TSelf OkAndFormat(IEnumerable<TData> unformattedData, TFilter filter, HttpStatusCode statusCode);
    /// <inheritdoc />
    static abstract TSelf Fail(TFilter filter, string error, string? errorCode = null, HttpStatusCode statusCode = HttpStatusCode.BadRequest);
}
