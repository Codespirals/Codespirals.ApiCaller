using System.Net;

namespace Codespirals.Solutions.ApiCaller
{
    public record ApiResult : IApiResult<ApiResult>
    {
        /// <inheritdoc cref="IApiResult{TSelf}.StatusCode"/>
        public HttpStatusCode StatusCode { get; } = HttpStatusCode.Ambiguous;
        /// <inheritdoc cref="IResult{TErrorCode}.Success"/>
        public bool Success { get; }
        /// <inheritdoc cref="IResult{TErrorCode}.ErrorCode"/>
        public string? ErrorCode { get; }
        /// <inheritdoc cref="IResult{TErrorCode}.Error"/>
        public string Error { get; } = "";

        private ApiResult(HttpStatusCode statusCode)
        {
            Success = true;
            StatusCode = statusCode;
        }
        private ApiResult(string error, string? errorCode, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            Success = false;
            StatusCode = statusCode;
            Error = error;
            ErrorCode = errorCode;
        }

        /// <inheritdoc cref="IResult{TSelf, TErrorCode}.Fail(string, TErrorCode?)"/>
        public static ApiResult Fail(string error, string? errorCode = null, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
            => new(error, errorCode, statusCode);
        public static ApiResult Ok(HttpStatusCode statusCode)
            => new(statusCode);
        public static ApiResult Short(IResult<string> result) => Fail(result.Error, result.ErrorCode);
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

        private ApiResult(TData data, HttpStatusCode statusCode)
        {
            Success = true;
            StatusCode = statusCode;
            Data = data;
        }
        private ApiResult(string error, string? errorCode, HttpStatusCode statusCode)
        {
            Success = false;
            StatusCode = statusCode;
            Error = error;
            ErrorCode = errorCode;
        }

        public static ApiResult<TData> Ok(TData data, HttpStatusCode statusCode)
            => new(data, statusCode);
        public static ApiResult<TData> Ok(TData data) => Ok(data, HttpStatusCode.OK);
        public static ApiResult<TData> Fail(string error, string? errorCode = null, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
            => new(error, errorCode, statusCode);
        public static ApiResult<TData> Short(IResult<string> result) => Fail(result.Error, result.ErrorCode);
    }
}
