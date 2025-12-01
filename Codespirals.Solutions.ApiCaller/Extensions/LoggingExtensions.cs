using Microsoft.Extensions.Logging;

namespace Codespirals.Solutions.ApiCaller
{
    internal static class LoggingExtensions
    {
        internal static IDisposable? BeginLoggingApiCall(this ILogger logger, string httpMethod, string baseUrl, string? slug = "")
        {
            Dictionary<string, string> tags = new()
            { { "Api", baseUrl }, { "Method", httpMethod } };
            if (!string.IsNullOrEmpty(slug)) { tags.Add("Endpoint", slug); }
            IDisposable? scope = logger.BeginScope(tags);
            logger.LogInformation("Starting Api call.");
            return scope;
        }
        internal static void LogApiRequest(this ILogger logger, HttpRequestMessage request)
            => logger.LogStep(Base.LoggingExtensions.State.InProgress, $"Sending {request.Method} request call to {request.RequestUri}\nWith data: {request.Content?.ReadAsStream().ToString() ?? "No data"}");
        internal static void LogApiResponse(this ILogger logger, HttpResponseMessage response)
        {
            Stream content = response.Content.ReadAsStream();
            logger.LogStep(Base.LoggingExtensions.State.InProgress, content.ToString() ?? "No Content");
        }
        internal static void LogApiResponseHeaders(this ILogger logger, HttpResponseMessage response)
        {
            string headers = string.Join("\n", response.Headers.Select(h => $"{h.Key}:{h.Value}"));
            logger.LogStep(Base.LoggingExtensions.State.InProgress, headers.ToString() ?? "No Headers");
        }
        internal static void LogApiSuccess(this ILogger logger)
        {
            logger.LogStep(Base.LoggingExtensions.State.Success, "Api Call sucessfully completed.");
        }
        internal static void LogApiFail(this ILogger logger, string? error)
        {
            logger.LogStep(Base.LoggingExtensions.State.Cancelled, $"Api Call failed: {error ?? "No error message"}");
        }
        internal static void LogApiException(this ILogger logger, Exception e)
        {
            logger.LogException(Base.LoggingExtensions.State.Stopped, e);
        }
    }
}
