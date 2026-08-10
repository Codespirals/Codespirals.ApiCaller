namespace Codespirals.Solutions.ApiCaller.Example
{
    public interface IApiExampleService
    {
        Task<CatFact?> GetCatFact();
    }

    public class ApiExampleService(IApiCallerFactory apiCallerFactory) : IApiExampleService
    {
        private readonly ApiCaller _apiService = apiCallerFactory.CreateApiCaller("https://catfact.ninja", userAgent:"api-caller-test");

        public async Task<CatFact?> GetCatFact()
        {
            var fact = await _apiService.Get<CatFact>("/fact");
            if (fact.Success)
            {
                return fact.Data;
            }
            return null;
        }
    }
}
