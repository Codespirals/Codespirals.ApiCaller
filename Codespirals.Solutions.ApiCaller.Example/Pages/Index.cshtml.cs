using Codespirals.Solutions.ApiCaller.Example;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Codespirals.ApiCaller.Example
{
    public class IndexModel(ILogger<IndexModel> logger, IApiExampleService apiExampleService) : PageModel
    {
        private readonly ILogger<IndexModel> _logger = logger;
        private readonly IApiExampleService _exampleService = apiExampleService;

        public string CatFact { get; set; } = Solutions.ApiCaller.Example.Resources.ExampleText.ErrorMessage;

        public async Task OnGetAsync()
        {
            CatFact = (await _exampleService.GetCatFact())?.Fact ?? Solutions.ApiCaller.Example.Resources.ExampleText.ErrorMessage;
        }
    }
}
