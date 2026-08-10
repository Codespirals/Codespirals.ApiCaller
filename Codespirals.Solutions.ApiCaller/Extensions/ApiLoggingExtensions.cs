using Codespirals.Base.Logging;
using Microsoft.Extensions.Logging;

namespace Codespirals.Solutions.ApiCaller;

internal static class ApiLoggingExtensions
{
    internal static IDisposable? BeginLoggingApiCall(this ILogger<ApiCaller> logger, string? httpMethod, string? endpoint)
    {
        (string, string)[] tags = [];
        if (httpMethod is not null)
            tags.Append(("Method", httpMethod));
        if (endpoint is not null)
            tags.Append(("Endpoint", endpoint));
        return logger.BeginLog(nameof(ApiCaller), message:"Starting API Call", tags);
    }
    internal static void LogApiRequest(this ILogger<ApiCaller> logger, HttpRequestMessage request)
        => logger.LogStep(State.InProgress, $"Sending {request.Method} request call to {request.RequestUri}\nWith data: {request.Content?.ReadAsStream().ToString() ?? "No data"}");
    internal static async Task LogApiResponse(this ILogger<ApiCaller> logger, HttpResponseMessage response)
    {
        string? content = await response.Content.ReadAsStringAsync();
        logger.LogStep(State.InProgress, $"Status Code: {(int)response.StatusCode}\nContent: {content}");
    }
    internal static void LogApiResponseHeaders(this ILogger<ApiCaller> logger, HttpResponseMessage response)
    {
        string headers = string.Join("\n", response.Headers.Select(h => $"{h.Key}:{h.Value}"));
        logger.LogStep(State.InProgress, headers.ToString() ?? "No Headers");
    }
    internal static void LogApiSuccess(this ILogger<ApiCaller> logger)
        => logger.LogStep(State.Success, "Api Call sucessfully completed.");
    internal static void LogApiFail(this ILogger<ApiCaller> logger, string? error = null)
        => logger.LogStep(State.Cancelled, $"Api Call failed: {error ?? "No error message"}");
    internal static void LogApiException(this ILogger<ApiCaller> logger, Exception e)
        => logger.LogException(State.Stopped, e);
}
