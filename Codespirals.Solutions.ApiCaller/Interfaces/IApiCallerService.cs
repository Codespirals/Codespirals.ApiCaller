using Codespirals.Base.Filtering;
using System.Net.Http.Headers;

namespace Codespirals.Solutions.ApiCaller;
/// <summary>
/// The interface depicting the main service that interacts with the Collabour API
/// </summary>
public interface IApiCallerService : INameable
{
    /// <summary>
    /// The base url of the API being called
    /// </summary>
    string BaseUrl { get; }

    /// <summary>
    /// Add a default header sent with every api call
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    void AddApiHeader(string name, string value);

    /// <summary>
    /// A lightweight request to the API with less overhead, however it has no application side logging and no feedback on failure
    /// </summary>
    /// <typeparam name="TData">The expected return type</typeparam>
    /// <param name="slug">A slug string (optional)</param>
    /// <returns>An item of the given type or <see langword="null"/>, if it fails</returns>
    Task<TData?> QuickGet<TData>(string slug = "", params List<KeyValuePair<string, string>> queryParameters);

    /// <summary>
    /// Send a create request to the API
    /// </summary>
    /// <typeparam name="TData">The expected return type</typeparam>
    /// <typeparam name="TBody">The type of the data being sent</typeparam>
    /// <param name="body">The body data</param>
    /// <param name="slug">A slug string (optional)</param>
    /// <returns>A result pattern item containing the requested item, or an explanation of why it failed.</returns>
    Task<ApiResult<TData>> Post<TData, TBody>(TBody body, string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters);

    /// <summary>
    /// Send an update request to the API
    /// </summary>
    /// <typeparam name="TData">The expected return type</typeparam>
    /// <typeparam name="TBody">The type of the data being sent</typeparam>
    /// <param name="body">The body data</param>
    /// <param name="slug">A slug string (optional)</param>
    /// <returns>A result pattern item containing the requested item, or an explanation of why it failed.</returns>
    Task<ApiResult<TData>> Put<TData, TBody>(TBody body, string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters);

    /// <summary>
    /// Send a patch request to the API
    /// Patch requests are generally used when an item is only partially replaced
    /// </summary>
    /// <typeparam name="TData">The expected return type</typeparam>
    /// <typeparam name="TBody">The type of the data being sent</typeparam>
    /// <param name="body">The body data</param>
    /// <param name="slug">A slug string (optional)</param>
    /// <returns>A result pattern item containing the requested item, or an explanation of why it failed.</returns>
    Task<ApiResult<TData>> Patch<TData, TBody>(TBody body, string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters);

    /// <summary>
    /// Send a request to get a resource to the API
    /// </summary>
    /// <typeparam name="TData">The expected return type</typeparam>
    /// <param name="slug">A slug string (optional)</param>
    /// <returns>A result pattern item containing the requested item, or an explanation of why it failed.</returns>
    Task<ApiResult<TData>> Get<TData>(string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters);

    /// <summary>
    /// Send a GET request to get a partial list of resources to the API
    /// </summary>
    /// <typeparam name="TData">The expected return type</typeparam>
    /// <param name="slug">A slug string (optional)</param>
    /// <returns>An api result the requested list.</returns>
    Task<ApiListResult<TData>> GetMany<TData>(string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters);

    /// <summary>
    /// Send a GET request to get a partial list of resources to the API
    /// </summary>
    /// <typeparam name="TData">The expected return type</typeparam>
    /// <param name="slug">A slug string (optional)</param>
    /// <returns>An api result with the requested section of the list as well as the total possible results found.</returns>
    Task<ApiFilteredListResult<TData, TFilterParameters>> GetManyFiltered<TData, TFilterParameters>(TFilterParameters paramters, string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
        where TFilterParameters : IFilterParameters, new();

    /// <summary>
    /// Send a GET request to get a partial list of resources to the API
    /// </summary>
    /// <typeparam name="TData">The expected return type</typeparam>
    /// <param name="slug">A slug string (optional)</param>
    /// <returns>An api result with the requested section of the list as well as the total possible results found.</returns>
    Task<ApiSearchResult<TData, TSearchParameters>> Search<TData, TSearchParameters>(TSearchParameters paramters, string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
        where TSearchParameters : ISearchParameters, new();

    /// <summary>
    /// Send a delete request to the API
    /// </summary>
    /// <param name="slug">A slug string (optional)</param>
    /// <returns>A result pattern item containing the requested item, or an explanation of why it failed.</returns>
    Task<ApiResult> Delete(string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters);

    /// <summary>
    /// Send a custom built request for edge cases this library doesn't address
    /// </summary>
    /// <typeparam name="TData">The expected result type</typeparam>
    /// <param name="request">The <see cref="HttpRequestMessage"/> - use the <see cref="HttpRequestBuilder"/> to easily build</param>
    /// <returns></returns>
    Task<ApiResult<TData>> CustomRequest<TData>(HttpRequestMessage request);

    /// <summary>
    /// Send a request create a tunnel to the API
    /// </summary>
    /// <typeparam name="TData">The expected return type</typeparam>
    /// <param name="port">The port that should be connected to</param>
    /// <param name="slug">A slug string (optional)</param>
    /// <returns>A result pattern item containing the requested item, or an explanation of why it failed.</returns>
    Task<ApiResult<TData>> Connect<TData>(int port, string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters);

    /// <summary>
    /// Send a request to get a head response to the API
    /// </summary>
    /// <param name="slug">A slug string (optional)</param>
    /// <returns>The <see cref="HttpHeaders"/> that would result from a <see cref="Get()"/> call, or <see langword="null"/>, if it fails catastrophically</returns>
    Task<HttpHeaders?> Head(string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters);

    /// <summary>
    /// Send a request to get the options of the API
    /// </summary>
    /// <param name="slug">A slug string (optional)</param>
    /// <returns>The options in a <see cref="HttpHeaderValueCollection{string}"/> of the given type or <see langword="null"/>, if it fails catastrophically</returns>
    Task<HttpHeaderValueCollection<string>?> Options(string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters);
}