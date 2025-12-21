namespace Codespirals.Solutions.ApiCaller;
/// <summary>
/// The interface depicting the main service that interacts with the Collabour API
/// </summary>
public interface IApiCallerFactory
{
    public ApiCaller InitializeApiCaller(string baseUrl);
}