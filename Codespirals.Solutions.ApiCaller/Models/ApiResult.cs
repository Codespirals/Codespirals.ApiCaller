using System.Net;

namespace Codespirals.Solutions.ApiCaller
{
    public record ApiResult : IApiResult<ApiResult>
    {
        public HttpStatusCode StatusCode { get; } = HttpStatusCode.Ambiguous;
        public bool Success { get; }
        public string? ErrorCode { get; }
        /// <inheritdoc />
        public string Error { get; } = "";

        private ApiResult(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }
        private ApiResult(string error, string? errorCode)
        {
            Success = false;
            Error = error;
            ErrorCode = errorCode;
        }
        private ApiResult(HttpStatusCode statusCode, string error, string? errorCode) : this(statusCode)
        {
            Success = false;
            Error = error;
            ErrorCode = errorCode;
        }

        public static ApiResult Fail(HttpStatusCode statusCode, string error, string? errorCode = null)
            => new(statusCode, error, errorCode);
        public static ApiResult Fail(string error, string? errorCode = null)
            => new(error, errorCode);
        public static ApiResult Ok(HttpStatusCode statusCode)
            => new(statusCode);
    }

    public record ApiResult<TData> : IApiResult<ApiResult<TData>, TData>
    {
        public HttpStatusCode StatusCode { get; } = HttpStatusCode.Ambiguous;
        public bool Success { get; }
        /// <inheritdoc />
        public TData? Data { get; }
        public string? ErrorCode { get; }
        /// <inheritdoc />
        public string Error { get; } = "";

        private ApiResult(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }
        private ApiResult(HttpStatusCode statusCode, TData data) : this(statusCode)
        {
            Success = true;
            Data = data;
        }
        private ApiResult(string error, string? errorCode)
        {
            Success = false;
            Error = error;
            ErrorCode = errorCode;
        }
        private ApiResult(HttpStatusCode statusCode, string error, string? errorCode) : this(statusCode)
        {
            Success = false;
            Error = error;
            ErrorCode = errorCode;
        }

        public static ApiResult<TData> Fail(HttpStatusCode statusCode, string error, string? errorCode = null)
            => new(statusCode, error, errorCode);
        public static ApiResult<TData> Fail(string error, string? errorCode = null)
            => new(error, errorCode);
        public static ApiResult<TData> Ok(HttpStatusCode statusCode)
            => new(statusCode);
        public static ApiResult<TData> Ok(HttpStatusCode statusCode, TData data)
            => new(statusCode, data);
    }
}
