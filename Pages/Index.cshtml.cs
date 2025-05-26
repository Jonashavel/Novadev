using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
//using Newtonsoft.Json;

namespace Novadev.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public IActionResult OnPostSearch(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Please enter a location to search.");
            }

            var result = new
            {
                query = query,
                places = new[]
                {
                    new { id = "1", name = "Bank A", address = "Street 1", phone = "123-456", latitude = 34.039737, longitude = -118.270293 },
                    new { id = "2", name = "ATM B", address = "Street 2", phone = "789-012", latitude = 34.040000, longitude = -118.271000 },
                }
            };

            return new JsonResult(result);
        }

        public IActionResult OnPostItemClicked(string id)
        {
            var details = new
            {
                id = id,
                name = id == "1" ? "Bank A" : "ATM B",
                address = id == "1" ? "Street 1" : "Street 2",
                phone = id == "1" ? "123-456" : "789-012",
                description = id == "1" ? "A well-known bank" : "A local ATM",
                latitude = id == "1" ? 34.039737 : 34.040000,
                longitude = id == "1" ? -118.270293 : -118.271000
            };

            return new JsonResult(details);
        }

        public void OnGet()
        {
        }
    }
}