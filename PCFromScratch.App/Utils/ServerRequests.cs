using System.Text.Json;

using PCFromScratch.DTOModels;

namespace PCFromScratch.App.Utils;

public class ServerRequests
{
    public readonly string ServerAddress;
    private readonly HttpClient _httpClient;
    private JsonSerializerOptions options;

    public ServerRequests()
    {
        ServerAddress = "http://192.168.0.77:5160";
        _httpClient = new HttpClient();
        options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<IEnumerable<T>?> GetItems<T>(string address)
    {
        var response = await _httpClient.GetAsync(ServerAddress + address);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<T>>(content, options);
        }
        return null;
    }

    public async Task<T?> GetItem<T>(string address, Guid id)
    {
        var response = await _httpClient.GetAsync($"{ServerAddress}{address}/{id}");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content, options);
        }
        return default;
    }

    public async Task<IEnumerable<OfferDtoModel>?> GetOffers(string address, Guid id)
    {
        var response = await _httpClient.GetAsync($"{ServerAddress}{address}/{id}/offers");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<OfferDtoModel>>(content, options);
        }
        return null;
    }
}