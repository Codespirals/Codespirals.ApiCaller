using Codespirals.Base.Filtering;
using Codespirals.Base.Results;
using System.Net;

namespace Codespirals.Solutions.ApiCaller
{
    public record ApiFilteredListResult<TData, TFilter> : IApiFilteredListResult<ApiFilteredListResult<TData, TFilter>, TData, TFilter>
        where TFilter : IFilterParameters, new()
    {
        public bool Success { get; }
        public TFilter Parameters { get; init; } = new TFilter();
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.Ambiguous;
        public int TotalResults { get; init; }
        public IEnumerable<TData>? Data { get; init; }
        public string? ErrorCode { get; }
        public string Error { get; } = "";

        private ApiFilteredListResult(IEnumerable<TData> formattedData, TFilter parameters, int totalResult, HttpStatusCode statusCode)
        {
            Success = true;
            StatusCode = statusCode;
            Data = formattedData;
            Parameters = parameters;
            TotalResults = totalResult;
        }
        private ApiFilteredListResult(string error, string? errorCode, HttpStatusCode statusCode)
        {
            Success = false;
            StatusCode = statusCode;
            Error = error;
            ErrorCode = errorCode;
        }
        private ApiFilteredListResult(TFilter parameters, string error, string? errorCode, HttpStatusCode statusCode) : this(error, errorCode, statusCode)
        {
            Parameters = parameters;
        }

        public static ApiFilteredListResult<TData, TFilter> Fail(string error, string? errorCode = null, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
            => new(error, errorCode, statusCode);
        public static ApiFilteredListResult<TData, TFilter> Fail(TFilter parameters, string error, string? errorCode = null, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
            => new(parameters, error, errorCode, statusCode);
        public static ApiFilteredListResult<TData, TFilter> Fail(TFilter filter, string error, string? errorCode = null)
            => Fail(filter, error, errorCode, HttpStatusCode.BadRequest);
        public static ApiFilteredListResult<TData, TFilter> Ok(IEnumerable<TData> formattedData, TFilter filter, int totalResults)
            => Ok(formattedData, filter, totalResults, HttpStatusCode.OK);
        public static ApiFilteredListResult<TData, TFilter> Ok(IEnumerable<TData> filteredData, TFilter parameters, int totalResults, HttpStatusCode statusCode)
            => new(filteredData, parameters, totalResults, statusCode);

        public static ApiFilteredListResult<TData, TFilter> Short(IResult<string> result) => Fail(result.Error, result.ErrorCode);
    }
}
