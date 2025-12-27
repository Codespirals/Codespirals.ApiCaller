namespace Codespirals.Solutions.ApiCaller.Example
{
    public interface IApiExampleService
    {
        Task<CatFact?> GetCatFact();
    }

    public class ApiExampleService(IApiCallerFactory apiCallerFactory) : IApiExampleService
    {
        private readonly ApiCaller _apiService = apiCallerFactory.InitializeApiCaller("https://catfact.ninja", "api-caller-test");

        public async Task<CatFact?> GetCatFact()
        {
            var fact = await _apiService.Post<CatFact>("fact");
            if (fact.Success)
            {
                return fact.Data;
            }
            return null;
        }
    }
}
