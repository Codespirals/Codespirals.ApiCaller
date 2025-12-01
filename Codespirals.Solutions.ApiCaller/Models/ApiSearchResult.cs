using System.Net;

namespace Codespirals.Solutions.ApiCaller
{
    public record ApiSearchResult<TData, TFilter> : IApiFilteredListResult<ApiSearchResult<TData, TFilter>, TData, TFilter>
        where TFilter : ISearchParameters, new()
    {
        /// <inheritdoc cref="IResult{TErrorCode}.Success"/>
        public bool Success { get; }

        /// <inheritdoc cref="IPagination{TParamters}.Parameters"/>
        public TFilter Parameters { get; init; } = new TFilter();
        /// <inheritdoc cref="IApiResult{TSelf}.StatusCode"/>
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.Ambiguous;
        /// <inheritdoc cref="IPagination{TParamters}.TotalResults"/>
        public int TotalResults { get; init; }
        /// <inheritdoc cref="IResultWithData{TSelf, TErrorCode, TData}.Data"/>
        public IEnumerable<TData>? Data { get; init; }
        /// <inheritdoc cref="IResult{TErrorCode}.Error"/>
        public string Error { get; } = "";
        /// <inheritdoc cref="IResult{TErrorCode}.ErrorCode"/>
        public string? ErrorCode { get; }

        private ApiSearchResult(IEnumerable<TData> unformattedData, TFilter parameters, HttpStatusCode statusCode)
        {
            Success = true;
            StatusCode = statusCode;
            Data = unformattedData.ApplyFilterParameters(parameters, short.MaxValue, out int totalResults);
            Parameters = parameters;
            TotalResults = totalResults;
        }
        private ApiSearchResult(IEnumerable<TData> formattedData, TFilter parameters, int totalResult, HttpStatusCode statusCode)
        {
            Success = true;
            StatusCode = statusCode;
            Data = formattedData;
            Parameters = parameters;
            TotalResults = totalResult;
        }
        private ApiSearchResult(string error, string? errorCode, HttpStatusCode statusCode)
        {
            Success = false;
            StatusCode = statusCode;
            Error = error;
            ErrorCode = errorCode;
        }
        private ApiSearchResult(TFilter parameters, string error, string? errorCode, HttpStatusCode statusCode) : this(error, errorCode, statusCode)
        {
            Parameters = parameters;
        }

        public static ApiSearchResult<TData, TFilter> Fail(string error, string? errorCode = null, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
            => new(error, errorCode, statusCode);
        public static ApiSearchResult<TData, TFilter> Fail(TFilter parameters, string error, string? errorCode = null, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
            => new(parameters, error, errorCode, statusCode);
        public static ApiSearchResult<TData, TFilter> Fail(TFilter filter, string error, string? errorCode = null)
            => Fail(filter, error, errorCode, HttpStatusCode.BadRequest);
        public static ApiSearchResult<TData, TFilter> Ok(IEnumerable<TData> formattedData, TFilter filter, int totalResults)
            => Ok(formattedData, filter, totalResults, HttpStatusCode.OK);
        public static ApiSearchResult<TData, TFilter> Ok(IEnumerable<TData> filteredData, TFilter parameters, int totalResults, HttpStatusCode statusCode)
            => new(filteredData, parameters, totalResults, statusCode);
        public static ApiSearchResult<TData, TFilter> OkAndFormat(IEnumerable<TData> unformattedData, TFilter filter)
            => OkAndFormat(unformattedData, filter, HttpStatusCode.OK);
        public static ApiSearchResult<TData, TFilter> OkAndFormat(IEnumerable<TData> unfliteredData, TFilter parameters, HttpStatusCode statusCode)
            => new(unfliteredData, parameters, statusCode);

        public static ApiSearchResult<TData, TFilter> Short(IResult<string> result) => Fail(result.Error, result.ErrorCode);
    }
}
