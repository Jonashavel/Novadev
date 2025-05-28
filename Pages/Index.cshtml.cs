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

        public async Task<IActionResult> OnGetSearchPOIAsync(double lat, double lon)
{
    var httpClient = _httpClientFactory.CreateClient();
    string subscriptionKey = "9L98ONwzVRUxJoCBRC0eS7LsA2RnB7ghI0qZfeIt5aVCRZzDykbFJQQJ99BEAC5RqLJZjxS7AAAgAZMP2QkN";
    int radius = 5000; // 5 km

    string url = $"https://atlas.microsoft.com/search/poi/category/json?api-version=1.0&subscription-key={subscriptionKey}&lat={lat}&lon={lon}&radius={radius}&categorySet=7323,7376";

    var response = await httpClient.GetAsync(url);
    if (!response.IsSuccessStatusCode)
    {
        return StatusCode((int)response.StatusCode, "Error fetching POIs.");
    }

    var json = await response.Content.ReadAsStringAsync();
    var data = JsonDocument.Parse(json);

    var results = data.RootElement.GetProperty("results");

    var pois = new List<object>();
    foreach (var result in results.EnumerateArray())
    {
        var position = result.GetProperty("position");
        var poi = result.GetProperty("poi");
        var address = result.GetProperty("address");

        pois.Add(new
        {
            name = poi.GetProperty("name").GetString(),
            lat = position.GetProperty("lat").GetDouble(),
            lon = position.GetProperty("lon").GetDouble(),
            address = address.GetProperty("freeformAddress").GetString()
        });
    }

    return new JsonResult(pois);
}

        public void OnGet()
        {
        }
    }
}
