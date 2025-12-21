using Microsoft.Extensions.Logging;

namespace Codespirals.Solutions.ApiCaller
{
    internal static class ApiLoggingExtensions
    {
        internal static IDisposable? BeginLoggingApiCall(this ILogger<ApiCallerFactory> logger, string? httpMethod, string? endpoint)
        {
            Dictionary<string, string> tags = [];
            if (httpMethod is not null)
                tags.Add("Method", httpMethod);
            if (endpoint is not null)
                tags.Add("Endpoint", endpoint);
            return logger.BeginLog(nameof(ApiCallerFactory), tags, "Starting API Call");
        }
        internal static void LogApiRequest(this ILogger<ApiCallerFactory> logger, HttpRequestMessage request)
            => logger.LogStep(LoggingExtensions.State.InProgress, $"Sending {request.Method} request call to {request.RequestUri}\nWith data: {request.Content?.ReadAsStream().ToString() ?? "No data"}");
        internal static void LogApiResponse(this ILogger<ApiCallerFactory> logger, HttpResponseMessage response)
        {
            string? content = response.Content.IsText() ? response.Content.ToString() : "No Content";
            logger.LogStep(LoggingExtensions.State.InProgress, $"Error Code: {response.StatusCode} Content: {content}");
        }
        internal static void LogApiResponseHeaders(this ILogger<ApiCallerFactory> logger, HttpResponseMessage response)
        {
            string headers = string.Join("\n", response.Headers.Select(h => $"{h.Key}:{h.Value}"));
            logger.LogStep(LoggingExtensions.State.InProgress, headers.ToString() ?? "No Headers");
        }
        internal static void LogApiSuccess(this ILogger<ApiCallerFactory> logger)
            => logger.LogStep(LoggingExtensions.State.Success, "Api Call sucessfully completed.");
        internal static void LogApiFail(this ILogger<ApiCallerFactory> logger, string? error = null)
            => logger.LogStep(LoggingExtensions.State.Cancelled, $"Api Call failed: {error ?? "No error message"}");
        internal static void LogApiException(this ILogger<ApiCallerFactory> logger, Exception e)
            => logger.LogException(LoggingExtensions.State.Stopped, e);
    }
}
