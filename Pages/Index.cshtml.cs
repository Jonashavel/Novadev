using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;

namespace Novadev.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        // Autocomplete API for city search
        public async Task<IActionResult> OnGetSearchCityAsync(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return BadRequest("Search term is required.");
            }

            var httpClient = _httpClientFactory.CreateClient();
            string subscriptionKey = "9L98ONwzVRUxJoCBRC0eS7LsA2RnB7ghI0qZfeIt5aVCRZzDykbFJQQJ99BEAC5RqLJZjxS7AAAgAZMP2QkN";
            string url = $"https://atlas.microsoft.com/search/address/json?api-version=1.0&subscription-key={subscriptionKey}&typeahead=true&query={term}&limit=5";

            var response = await httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Error fetching city data.");
            }

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonDocument.Parse(json);

            var results = data.RootElement.GetProperty("results");

            var suggestions = new List<object>();
            foreach (var result in results.EnumerateArray())
            {
                var address = result.GetProperty("address");
                var position = result.GetProperty("position");

                suggestions.Add(new
                {
                    label = address.GetProperty("freeformAddress").GetString(),
                    value = address.GetProperty("freeformAddress").GetString(),
                    lat = position.GetProperty("lat").GetDouble(),
                    lon = position.GetProperty("lon").GetDouble()
                });
            }

            return new JsonResult(suggestions);
        }

        public void OnGet()
        {
        }
    }
}
