using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using System;

namespace Games.Services
{

    // A service that retrieves game cover images from the RAWG API.
    public class GameImageService : IGameImageService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GameImageService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["Rawg:ApiKey"]
                ?? throw new InvalidOperationException("RAWG API key not configured.");
        }

        public async Task<string?> GetGameImageUrlAsync(string gameName)
        {
            if (string.IsNullOrWhiteSpace(gameName))
                return null;

            var query = gameName.Replace(' ', '-');
            var url = $"games?search={query}&page_size=1&key={_apiKey}";

            try
            {
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                if (root.TryGetProperty("results", out var results) && results.GetArrayLength() > 0)
                {
                    var game = results[0];
                    if (game.TryGetProperty("background_image", out var image))
                        return image.GetString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching image for '{gameName}': {ex.Message}");
            }

            return null;
        }
    }
}
