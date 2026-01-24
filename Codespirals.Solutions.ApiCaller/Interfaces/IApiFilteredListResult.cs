using Codespirals.Base.Filtering;
using Codespirals.Base.Results;
using System.Net;

namespace Codespirals.Solutions.ApiCaller;
/// <summary>
/// Defines the result of an API operation that returns a filtered list of data, including filtering parameters and
/// pagination information.
/// </summary>
/// <typeparam name="TSelf">The concrete type implementing this interface.</typeparam>
/// <typeparam name="TData">The type of data items in the result list.</typeparam>
/// <typeparam name="TFilter">The type of filter parameters used to filter the data.</typeparam>
public interface IApiFilteredListResult<TSelf, TData, TFilter> : IApiResult, IFilteredListResult<string, TData, TFilter>
    where TSelf : IApiFilteredListResult<TSelf, TData, TFilter>
    where TFilter : IFilterParameters
{
    /// <summary>
    /// Create a successful result with a list of data filtered by the Filter parameters
    /// </summary>
    /// <param name="formattedData">The result data</param>
    /// <param name="filter">The parameters to filter by</param>
    /// <param name="totalResults">The number of the total possible result without filter. This enables pagination</param>
    /// <param name="statusCode">The HTTP status code to associate with the result. The default is <see cref="HttpStatusCode.OK"/>.</param>
    /// <returns></returns>
    static abstract TSelf Ok(IEnumerable<TData> formattedData, TFilter filter, int totalResults, HttpStatusCode statusCode = HttpStatusCode.OK);

    /// <summary>
    /// Create a failed result returning with the Filter parameters and information about the error
    /// </summary>
    /// <param name="filter">The filter parameters that were sent with the request</param>
    /// <param name="error">An error message</param>
    /// <param name="errorCode">An optional error code</param>
    /// <param name="statusCode">The HTTP status code to associate with the result. The default is <see cref="HttpStatusCode.OK"/>.</param>
    /// <returns></returns>
    static abstract TSelf Fail(TFilter filter, string error, string? errorCode = null, HttpStatusCode statusCode = HttpStatusCode.BadRequest);
}
