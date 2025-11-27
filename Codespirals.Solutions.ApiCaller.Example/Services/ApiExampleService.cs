using Codespirals.Solutions.ApiCaller;

namespace Codespirals.Solutions.ApiCaller.Example
{
    public interface IApiExampleService
    {
        Task<CatFact?> GetCatFact();
    }

    public class ApiExampleService([FromKeyedServices("CatFactApi")] IApiCallerService apiService) : IApiExampleService
    {
        private readonly IApiCallerService _apiService = apiService;

        public async Task<CatFact?> GetCatFact()
        {
            var fact = await _apiService.Get<CatFact>("fact");
            if (fact.Success)
            {
                return fact.Data;
            }
            return null;
        }
    }
}
