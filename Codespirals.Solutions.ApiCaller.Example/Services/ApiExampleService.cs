namespace Codespirals.ApiCaller.Example
{
    public interface IApiExampleService
    {
        Task<CatFact?> GetCatFact();
    }

    public class ApiExampleService([FromKeyedServices("CatFactApi")] IApiService apiService) : IApiExampleService
    {
        private readonly IApiService _apiService = apiService;

        public async Task<CatFact?> GetCatFact()
            => await _apiService.Get<CatFact>("fact");
    }
}
