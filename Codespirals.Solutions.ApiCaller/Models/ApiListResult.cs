using Codespirals.Base.Results;
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

        private ApiListResult(IEnumerable<TData> data, HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
            Success = true;
            Data = data;
        }
        private ApiListResult(string error, string? errorCode, HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
            Success = false;
            Error = error;
            ErrorCode = errorCode;
        }

        public static ApiListResult<TData> Fail(string error, string? errorCode = null, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
            => new(error, errorCode, statusCode);
        public static ApiListResult<TData> Ok(IEnumerable<TData> data, HttpStatusCode statusCode = HttpStatusCode.OK)
            => new(data, statusCode);
        public static ApiListResult<TData> Ok(IEnumerable<TData> data) => Ok(data, HttpStatusCode.OK);
        public static ApiListResult<TData> Short(IResult<string> result) => Fail(result.Error, result.ErrorCode);
    }
}
