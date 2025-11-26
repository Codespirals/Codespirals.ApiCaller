using System.Net;

namespace Codespirals.Solutions.ApiCaller
{
    public record ApiFilteredListResult<TFilter, TData> : IApiFilteredListResult<ApiFilteredListResult<TFilter, TData>, TFilter, TData>
        where TFilter : IFilterParameters, new()
    {
        public bool Success { get; }
        public TFilter Parameters { get; init; } = new TFilter();
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.Ambiguous;
        public int TotalResults { get; init; }
        public IEnumerable<TData>? Data { get; init; }
        public string? ErrorCode { get; }
        public string Error { get; } = "";

        private ApiFilteredListResult(TFilter parameters, HttpStatusCode statusCode)
        {
            Parameters = parameters;
            StatusCode = statusCode;
        }
        private ApiFilteredListResult(TFilter parameters, HttpStatusCode statusCode, IEnumerable<TData> unformattedData) : this(parameters, statusCode)
        {
            Success = true;
            Data = unformattedData.ApplyFilterParameters(parameters, short.MaxValue, out int totalResults);
            TotalResults = totalResults;
        }
        private ApiFilteredListResult(TFilter parameters, HttpStatusCode statusCode, IEnumerable<TData> formattedData, int totalResult) : this(parameters, statusCode)
        {
            Success = true;
            Data = formattedData;
            TotalResults = totalResult;
        }
        private ApiFilteredListResult(string error, string? errorCode)
        {
            Success = false;
            Error = error;
            ErrorCode = errorCode;
        }
        private ApiFilteredListResult(TFilter parameters, HttpStatusCode statusCode, string error, string? errorCode) : this(error, errorCode)
        {
            Parameters = parameters;
            StatusCode = statusCode;
        }

        public static ApiFilteredListResult<TFilter, TData> Fail(string error, string? errorCode = null)
            => new(error, errorCode);
        public static ApiFilteredListResult<TFilter, TData> Fail(TFilter parameters, HttpStatusCode statusCode, string error, string? errorCode = null)
            => new(parameters, statusCode, error, errorCode);
        public static ApiFilteredListResult<TFilter, TData> Ok(TFilter parameters, HttpStatusCode statusCode, IEnumerable<TData> filteredData, int totalResults)
            => new(parameters, statusCode, filteredData, totalResults);
        public static ApiFilteredListResult<TFilter, TData> OkAndFormat(TFilter parameters, HttpStatusCode statusCode, IEnumerable<TData> unfliteredData)
            => new(parameters, statusCode, unfliteredData);
    }
}
