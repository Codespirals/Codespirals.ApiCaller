using Codespirals.Base.Filtering;
using Codespirals.Base.Results;
using System.Net;

namespace Codespirals.Solutions.ApiCaller
{
    /// <inheritdoc cref="IApiFilteredListResult{TSelf, TData, TFilter}"/> 
    public record ApiFilteredListResult<TData, TFilter> : IApiFilteredListResult<ApiFilteredListResult<TData, TFilter>, TData, TFilter>
        where TFilter : IFilterParameters, new()
    {
        /// <inheritdoc />
        public bool Success { get; }
        /// <inheritdoc />
        public TFilter Parameters { get; init; } = new TFilter();
        /// <inheritdoc />
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.Ambiguous;
        /// <inheritdoc />
        public int TotalResults { get; init; }
        /// <inheritdoc />
        public IEnumerable<TData>? Data { get; init; }
        /// <inheritdoc />
        public string? ErrorCode { get; }
        /// <inheritdoc />
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
        /// <inheritdoc cref="IApiResult{TSelf}.Fail(string, string?, HttpStatusCode)"/>
        public static ApiFilteredListResult<TData, TFilter> Fail(TFilter parameters, string error, string? errorCode = null, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
            => new(parameters, error, errorCode, statusCode);
        /// <inheritdoc cref="IResultWithData{TSelf, TErrorCode, TData}.Ok(TData)"/>
        public static ApiFilteredListResult<TData, TFilter> Ok(IEnumerable<TData> filteredData, TFilter parameters, int totalResults, HttpStatusCode statusCode = HttpStatusCode.OK)
            => new(filteredData, parameters, totalResults, statusCode);

        /// <inheritdoc cref="IResult{TSelf, TErrorCode}.Short(IResult{TErrorCode})"/>
        public static ApiFilteredListResult<TData, TFilter> Short(IResult<string> result) 
            => new(result.Error, result.ErrorCode, HttpStatusCode.BadRequest);
    }
}
