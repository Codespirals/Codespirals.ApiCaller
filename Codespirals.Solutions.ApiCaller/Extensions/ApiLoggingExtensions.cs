using Microsoft.Extensions.Logging;

namespace Codespirals.Solutions.ApiCaller
{
    internal static class ApiLoggingExtensions
    {
        internal static IDisposable? BeginLoggingApiCall(this ILogger logger, string httpMethod, string baseUrl, string? slug = "")
        {
            Dictionary<string, string> tags = new()
            { { "Api", baseUrl }, { "Method", httpMethod } };
            if (!string.IsNullOrEmpty(slug)) { tags.Add("Endpoint", slug); }
            return logger.BeginLog(nameof(ApiCallerService), tags, "Starting API Call");
        }
        internal static void LogApiRequest(this ILogger logger, HttpRequestMessage request)
            => logger.LogStep(LoggingExtensions.State.InProgress, $"Sending {request.Method} request call to {request.RequestUri}\nWith data: {request.Content?.ReadAsStream().ToString() ?? "No data"}");
        internal static void LogApiResponse(this ILogger logger, HttpResponseMessage response)
        {
            var content = response.Content.IsText() ? response.Content.ToString() : "No Content";
            logger.LogStep(LoggingExtensions.State.InProgress, $"Error Code: {response.StatusCode} Content: {content}");
        }
        internal static void LogApiResponseHeaders(this ILogger logger, HttpResponseMessage response)
        {
            string headers = string.Join("\n", response.Headers.Select(h => $"{h.Key}:{h.Value}"));
            logger.LogStep(LoggingExtensions.State.InProgress, headers.ToString() ?? "No Headers");
        }
        internal static void LogApiSuccess(this ILogger logger)
            => logger.LogStep(LoggingExtensions.State.Success, "Api Call sucessfully completed.");
        internal static void LogApiFail(this ILogger logger, string? error)
            => logger.LogStep(LoggingExtensions.State.Cancelled, $"Api Call failed: {error ?? "No error message"}");
        internal static void LogApiException(this ILogger logger, Exception e)
            => logger.LogException(LoggingExtensions.State.Stopped, e);
    }
}
