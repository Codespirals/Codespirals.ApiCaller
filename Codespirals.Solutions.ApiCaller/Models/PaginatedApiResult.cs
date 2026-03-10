using Codespirals.Base.Filtering;
using Codespirals.Base.Results;
using System.Net;

namespace Codespirals.Solutions.ApiCaller
{
    /// <summary>
    /// The result of an API operation that returns a filtered list of data, including filtering parameters
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TFilter"></typeparam>
    public record PaginatedApiResult<TData, TFilter> : IPaginatedApiResult<PaginatedApiResult<TData, TFilter>, TData, TFilter>
        where TFilter : IFilterParameters, new()
    {
        /// <inheritdoc cref="IResult{TErrorCode}.Success"/>
        public bool Success { get; }

        /// <inheritdoc cref="IPagination{TParamters}.Parameters"/>
        public TFilter Parameters { get; init; } = new TFilter();
        /// <inheritdoc cref="IApiResult.StatusCode"/>
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.Ambiguous;
        /// <inheritdoc cref="IPagination{TParamters}.TotalResults"/>
        public int TotalResults { get; init; }
        /// <inheritdoc cref="IHasData{TData}.Data"/>
        public IEnumerable<TData>? Data { get; init; }
        /// <inheritdoc cref="IResult{TErrorCode}.Error"/>
        public string Error { get; } = "";
        /// <inheritdoc cref="IResult{TErrorCode}.ErrorCode"/>
        public string? ErrorCode { get; }

        private PaginatedApiResult(IEnumerable<TData> formattedData, TFilter parameters, int totalResult, HttpStatusCode statusCode)
        {
            Success = true;
            StatusCode = statusCode;
            Data = formattedData;
            Parameters = parameters;
            TotalResults = totalResult;
        }
        private PaginatedApiResult(string error, string? errorCode, HttpStatusCode statusCode)
        {
            Success = false;
            StatusCode = statusCode;
            Error = error;
            ErrorCode = errorCode;
        }
        private PaginatedApiResult(TFilter parameters, string error, string? errorCode, HttpStatusCode statusCode) : this(error, errorCode, statusCode)
        {
            Parameters = parameters;
        }
        /// <inheritdoc />
        public static PaginatedApiResult<TData, TFilter> Fail(TFilter parameters, string error, string? errorCode = null, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
            => new(parameters, error, errorCode, statusCode);

        /// <inheritdoc />
        public static PaginatedApiResult<TData, TFilter> Ok(IEnumerable<TData> filteredData, TFilter parameters, int totalResults, HttpStatusCode statusCode = HttpStatusCode.OK)
            => new(filteredData, parameters, totalResults, statusCode);

        /// <inheritdoc cref="IResult{TSelf, TErrorCode}.Short(IResult{TErrorCode})"/>
        public static PaginatedApiResult<TData, TFilter> Short(IResult<string> result) 
            => new(result.Error, result.ErrorCode, HttpStatusCode.BadRequest);
    }
}
