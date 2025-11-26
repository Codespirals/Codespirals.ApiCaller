using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Codespirals.ApiCaller.Example
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IApiExampleService _exampleService;

        public string CatFact { get; set; } = Solutions.ApiCaller.Example.Resources.ExampleText.ErrorMessage;

        public IndexModel(ILogger<IndexModel> logger, IApiExampleService apiExampleService)
        {
            _logger = logger;
            _exampleService = apiExampleService;
        }

        public async Task OnGetAsync()
        {
            CatFact = (await _exampleService.GetCatFact())?.Fact ?? Solutions.ApiCaller.Example.Resources.ExampleText.ErrorMessage;
        }
    }
}
