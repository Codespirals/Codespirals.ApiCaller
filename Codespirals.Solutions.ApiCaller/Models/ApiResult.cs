using Codespirals.Base.Results;
using System.Net;

namespace Codespirals.Solutions.ApiCaller
{
    /// <inheritdoc cref="IApiResult{TSelf}"/>
    public record ApiResult : IApiResult<ApiResult>
    {
        /// <inheritdoc cref="IApiResult.StatusCode"/>
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

        /// <summary>
        /// Creates a successful <see cref="ApiResult"/> with the specified HTTP status code.
        /// </summary>
        /// <param name="statusCode">The HTTP status code to associate with the result. The default is <see cref="HttpStatusCode.OK"/>.</param>
        /// <returns>An <see cref="ApiResult"/> representing a successful operation with the given status code.</returns>
        public static ApiResult Ok(HttpStatusCode statusCode = HttpStatusCode.OK)
            => new(statusCode);

        /// <inheritdoc cref="IApiResult{TSelf}.Fail(string, string?, HttpStatusCode)"/>
        public static ApiResult Fail(string error, string? errorCode = null, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
            => new(error, errorCode, statusCode);

        /// <inheritdoc cref="IResult{TSelf, TErrorCode}.Short(IResult{TErrorCode})"/>
        public static ApiResult Short(IResult<string> result) => Fail(result.Error, result.ErrorCode);
    }

    /// <inheritdoc cref="IApiResult{TSelf, TData}"/>
    public record ApiResult<TData> : IApiResult<ApiResult<TData>, TData>
    {
        /// <inheritdoc cref="IApiResult.StatusCode"/>
        public HttpStatusCode StatusCode { get; } = HttpStatusCode.Ambiguous;
        /// <inheritdoc cref="IResult{T}.Success"/>
        public bool Success { get; }
        /// <inheritdoc cref="IHasData{TData}.Data"/>
        public TData? Data { get; }
        /// <inheritdoc cref="IResult{TErrorCode}.ErrorCode"/>
        public string? ErrorCode { get; }
        /// <inheritdoc cref="IResult{TErrorCode}.Error"/>
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

        /// <inheritdoc cref="IApiResult{TSelf, TData}.Ok(TData, HttpStatusCode)"/>
        public static ApiResult<TData> Ok(TData data, HttpStatusCode statusCode)
            => new(data, statusCode);
        /// <inheritdoc cref="IResultWithData{TSelf, TErrorCode, TData}.Ok(TData)"/>
        public static ApiResult<TData> Ok(TData data) => Ok(data, HttpStatusCode.OK);
        /// <inheritdoc cref="IApiResult{TSelf}.Fail(string, string?, HttpStatusCode)"/>
        public static ApiResult<TData> Fail(string error, string? errorCode = null, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
            => new(error, errorCode, statusCode);
        /// <inheritdoc cref="IResult{TSelf, TErrorCode}.Short(IResult{TErrorCode})"/>
        public static ApiResult<TData> Short(IResult<string> result) => Fail(result.Error, result.ErrorCode);
    }
}
