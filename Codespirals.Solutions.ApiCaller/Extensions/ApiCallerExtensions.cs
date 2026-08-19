using System.Net.Http.Json;

namespace Codespirals.Solutions.ApiCaller;

/// <summary>
/// A couple of extensions to call an URL direclty, bypassing your set <see cref="HttpClient.BaseAddress"/>
/// </summary>
public static class ApiCallerExtensions
{
    /// <summary>
    /// Make a <see cref="HttpMethod.Get"/> call to an absolute URL, bypassing your set <see cref="HttpClient.BaseAddress"/>
    /// </summary>
    /// <param name="caller"></param>
    /// <param name="url">The absolute url to call</param>
    /// <typeparam name="TData">The type of the requested data</typeparam>
    /// <returns>An <see cref="ApiResult"/> containing the requested data</returns>
    public static async Task<ApiResult<TData>> GetFromUrl<TData>(this ApiCaller caller, string url)
    {
        using var log = caller._logger.BeginLoggingApiCall(HttpMethod.Get.ToString(), url);
        using var tempClient = new HttpClient();
        var result = await tempClient.GetFromJsonAsync<TData>(url);
        if (result is null)
        {
            var errorMessage = $"Couldn't get data from {url}";
            caller._logger.LogApiFail(errorMessage);
            return ApiResult<TData>.Fail(errorMessage);
        }
        caller._logger.LogApiSuccess();
        return ApiResult<TData>.Ok(result);
    }
    /// <summary>
    /// Make a <see cref="HttpMethod.Post"/> call to an absolute URL, bypassing your set <see cref="HttpClient.BaseAddress"/>
    /// </summary>
    /// <param name="caller"></param>
    /// <param name="url">The absolute url to call</param>
    /// <param name="body">The body</param>
    /// <typeparam name="TData">The type of the requested data</typeparam>
    /// <typeparam name="TBody">The body of the request</typeparam>
    /// <returns>An <see cref="ApiResult"/> containing the requested data</returns>
    public static async Task<ApiResult<TData>> PostFromUrl<TData, TBody>(this ApiCaller caller, string url, TBody body)
    {
        using var log = caller._logger.BeginLoggingApiCall(HttpMethod.Get.ToString(), url);
        using var tempClient = new HttpClient();
        var result = await tempClient.PostAsJsonAsync(url, body);
        if (result is null)
        {
            var errorMessage = $"Couldn't get data from {url}";
            caller._logger.LogApiFail(errorMessage);
            return ApiResult<TData>.Fail(errorMessage);
        }
        var data = await result.Content.ReadFromJsonAsync<TData>();
        if (data is null)
        {
            var errorMessage = $"Couldn't get data from {url}";
            caller._logger.LogApiFail(errorMessage);
            return ApiResult<TData>.Fail(errorMessage);
        }
        caller._logger.LogApiSuccess();
        return ApiResult<TData>.Ok(data);
    }
    /// <summary>
    /// Make a <see cref="HttpMethod.Put"/> call to an absolute URL, bypassing your set <see cref="HttpClient.BaseAddress"/>
    /// </summary>
    /// <param name="caller"></param>
    /// <param name="url">The absolute url to call</param>
    /// <param name="body">The body</param>
    /// <typeparam name="TData">The type of the requested data</typeparam>
    /// <typeparam name="TBody">The body of the request</typeparam>
    /// <returns>An <see cref="ApiResult"/> containing the requested data</returns>
    public static async Task<ApiResult<TData>> PutFromUrl<TData, TBody>(this ApiCaller caller, string url, TBody body)
    {
        using var log = caller._logger.BeginLoggingApiCall(HttpMethod.Get.ToString(), url);
        using var tempClient = new HttpClient();
        var result = await tempClient.PutAsJsonAsync(url, body);
        if (result is null)
        {
            var errorMessage = $"Couldn't get data from {url}";
            caller._logger.LogApiFail(errorMessage);
            return ApiResult<TData>.Fail(errorMessage);
        }
        var data = await result.Content.ReadFromJsonAsync<TData>();
        if (data is null)
        {
            var errorMessage = $"Couldn't get data from {url}";
            caller._logger.LogApiFail(errorMessage);
            return ApiResult<TData>.Fail(errorMessage);
        }
        caller._logger.LogApiSuccess();
        return ApiResult<TData>.Ok(data);
    }
    /// <summary>
    /// Make a <see cref="HttpMethod.Patch"/> call to an absolute URL, bypassing your set <see cref="HttpClient.BaseAddress"/>
    /// </summary>
    /// <param name="caller"></param>
    /// <param name="url">The absolute url to call</param>
    /// <param name="body">The body</param>
    /// <typeparam name="TData">The type of the requested data</typeparam>
    /// <typeparam name="TBody">The body of the request</typeparam>
    /// <returns>An <see cref="ApiResult"/> containing the requested data</returns>
    public static async Task<ApiResult<TData>> PatchFromUrl<TData, TBody>(this ApiCaller caller, string url, TBody body)
    {
        using var log = caller._logger.BeginLoggingApiCall(HttpMethod.Get.ToString(), url);
        using var tempClient = new HttpClient();
        var result = await tempClient.PatchAsJsonAsync(url, body);
        if (result is null)
        {
            var errorMessage = $"Couldn't get data from {url}";
            caller._logger.LogApiFail(errorMessage);
            return ApiResult<TData>.Fail(errorMessage);
        }
        var data = await result.Content.ReadFromJsonAsync<TData>();
        if (data is null)
        {
            var errorMessage = $"Couldn't get data from {url}";
            caller._logger.LogApiFail(errorMessage);
            return ApiResult<TData>.Fail(errorMessage);
        }
        caller._logger.LogApiSuccess();
        return ApiResult<TData>.Ok(data);
    }
    /// <summary>
    /// Make a <see cref="HttpMethod.Delete"/> call to an absolute URL, bypassing your set <see cref="HttpClient.BaseAddress"/>
    /// </summary>
    /// <param name="caller"></param>
    /// <param name="url">The absolute url to call</param>
    /// <typeparam name="TData">The type of the requested data</typeparam>
    /// <returns>An <see cref="ApiResult"/> containing the requested data</returns>
    public static async Task<ApiResult<TData>> DeleteFromUrl<TData>(this ApiCaller caller, string url)
    {
        using var log = caller._logger.BeginLoggingApiCall(HttpMethod.Get.ToString(), url);
        using var tempClient = new HttpClient();
        var result = await tempClient.DeleteFromJsonAsync<TData>(url);
        if (result is null)
        {
            var errorMessage = $"Couldn't get data from {url}";
            caller._logger.LogApiFail(errorMessage);
            return ApiResult<TData>.Fail(errorMessage);
        }
        caller._logger.LogApiSuccess();
        return ApiResult<TData>.Ok(result);
    }
}
