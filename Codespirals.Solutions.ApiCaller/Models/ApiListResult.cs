using System.Net;

namespace Codespirals.Solutions.ApiCaller
{
    public record ApiListResult<TData> : IApiListResult<ApiListResult<TData>, TData>
    {
        public bool Success { get; }
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.Ambiguous;
        public int TotalResults { get; init; }
        public IEnumerable<TData>? Data { get; init; }
        public string? ErrorCode { get; }
        public string Error { get; } = "";

        private ApiListResult(HttpStatusCode statusCode, IEnumerable<TData> data)
        {
            StatusCode = statusCode;
            Success = true;
            Data = data;
        }
        private ApiListResult(string error, string? errorCode)
        {
            StatusCode = HttpStatusCode.BadRequest;
            Success = false;
            Error = error;
            ErrorCode = errorCode;
        }
        private ApiListResult(HttpStatusCode statusCode, string error, string? errorCode) : this(error, errorCode)
        {
            StatusCode = statusCode;
        }

        public static ApiListResult<TData> Fail(string error, string? errorCode = null)
            => new(error, errorCode);
        public static ApiListResult<TData> Fail(HttpStatusCode statusCode, string error, string? errorCode = null)
            => new(statusCode, error, errorCode);
        public static ApiListResult<TData> Ok(HttpStatusCode statusCode, IEnumerable<TData> data)
            => new(statusCode, data);
        public static ApiListResult<TData> Ok(IEnumerable<TData> data) => Ok(HttpStatusCode.OK, data);
        public static ApiListResult<TData> Short(IResult<string> result) => Fail(result.Error, result.ErrorCode);
    }
}
