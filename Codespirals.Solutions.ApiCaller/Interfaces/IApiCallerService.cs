using Codespirals.Base.Filtering;
using Codespirals.Base.Results;
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
    void AddDefaultHeader(string name, string value);

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
    /// <returns>An api result with the requested section of the list as well as the total possible results found.</returns>
    Task<ApiFilteredListResult<TData, TFilterParameters>> GetPaginated<TData, TResponse, TFilterParameters>(TFilterParameters paramters, string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
        where TFilterParameters : IFilterParameters, new()
        where TResponse : IPagination<TFilterParameters>, IHasData<IEnumerable<TData>>;

    /// <summary>
    /// Send a GET request to get a partial list of resources to the API
    /// </summary>
    /// <typeparam name="TData">The expected return type</typeparam>
    /// <param name="slug">A slug string (optional)</param>
    /// <returns>An api result with the requested section of the list as well as the total possible results found.</returns>
    Task<ApiSearchResult<TData, TSearchParameters>> Search<TData, TResponse, TSearchParameters>(TSearchParameters paramters, string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters)
        where TSearchParameters : ISearchParameters, new()
        where TResponse : IPagination<TSearchParameters>, IHasData<IEnumerable<TData>>;

    /// <summary>
    /// Send a delete request to the API
    /// </summary>
    /// <param name="slug">A slug string (optional)</param>
    /// <returns>A result pattern item containing the requested item, or an explanation of why it failed.</returns>
    Task<ApiResult> Delete(string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters);

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
    Task<ApiResult<HttpHeaders>> Head(string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters);

    /// <summary>
    /// Send a request to get the options of the API
    /// </summary>
    /// <param name="slug">A slug string (optional)</param>
    /// <returns>The options in a <see cref="HttpHeaderValueCollection{string}"/> of the given type or <see langword="null"/>, if it fails catastrophically</returns>
    Task<ApiResult<HttpHeaderValueCollection<string>>> Options(string slug = "", params List<KeyValuePair<string, string>> additionalQueryParameters);
}