using System.Net.Http.Json;
using System.Text.Json;

using Microsoft.Extensions.Configuration;

using PCFromScratch.Common;
using PCFromScratch.DTOModels;

namespace PCFromScratch.App.Utils;

/// <summary>
/// Handles all HTTP requests to the backend server.
/// </summary>
public class ServerRequests
{
    public readonly string ServerAddress;
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _options;

    public ServerRequests(IConfiguration configuration)
    {
        ServerAddress = "http://"+configuration["settings:serverIp"]+":5160";
        _httpClient = new HttpClient();
        _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    /// <summary>
    /// Retrieves a collection of items from a specified endpoint.
    /// </summary>
    /// <typeparam name="T">The type of items to retrieve.</typeparam>
    /// <param name="address">The API endpoint address.</param>
    /// <returns>Items, or null if the request fails.</returns>
    public async Task<IEnumerable<T>?> GetItems<T>(string address)
    {
        var response = await _httpClient.GetAsync(ServerAddress + address);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<T>>(content, _options);
        }
        return null;
    }

    /// <summary>
    /// Retrieves a single item by its ID from a specified endpoint.
    /// </summary>
    /// <typeparam name="T">The type of the item to retrieve.</typeparam>
    /// <param name="address">The API endpoint address.</param>
    /// <param name="id">The ID of the item.</param>
    /// <returns>Item, or default if not found or the request fails.</returns>
    public async Task<T?> GetItem<T>(string address, Guid id)
    {
        var response = await _httpClient.GetAsync($"{ServerAddress}{address}/{id}");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content, _options);
        }
        return default;
    }

    /// <summary>
    /// Retrieves a list of offers for a specific item.
    /// </summary>
    /// <param name="address">The API endpoint address for the item type.</param>
    /// <param name="id">The ID of the item.</param>
    /// <returns>Offers, or null if the request fails.</returns>
    public async Task<IEnumerable<OfferDtoModel>?> GetOffers(string address, Guid id)
    {
        var response = await _httpClient.GetAsync($"{ServerAddress}{address}/{id}/offers");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<OfferDtoModel>>(content, _options);
        }
        return null;
    }

    /// <summary>
    /// Sends a PC configuration to the server for a compatibility check.
    /// </summary>
    /// <param name="pc">The PC configuration to check.</param>
    /// <returns>Warnings, or null if the request fails.</returns>
    public async Task<IEnumerable<Warning>?> CheckPc(PcDtoModel pc)
    {
        var send = await _httpClient.PostAsJsonAsync($"{ServerAddress}/pc/check",pc);
        if (!send.IsSuccessStatusCode) return null;
        var content = await send.Content.ReadAsStringAsync();
        var list = JsonSerializer.Deserialize<List<WarningDtoModel>>(content, _options);
        var result = new List<Warning>();
        foreach (var warning in list ?? [])
        {
            result.Add(new Warning(warning.Severity.Value, warning.Message));
        }
        return result;
    }
    
    /// <summary>
    /// Checks if a PC configuration meets specified system requirements.
    /// </summary>
    /// <param name="pc">The PC configuration.</param>
    /// <param name="systemRequirements">The system requirements.</param>
    /// <returns>Bool - is pc fit requirements, dictionary - if not then why, or null if the request fails.</returns>
    public async Task<RequirementsResultDtoModel?> CheckRequirements(PcDtoModel pc,
        SystemRequirementsDtoModel systemRequirements)
    {
        CompareRequirementsRequest body = new (pc, systemRequirements);
        var send = await _httpClient.PostAsJsonAsync($"{ServerAddress}/pc/compare/requirements", body);
        if (!send.IsSuccessStatusCode) return null;
        var content = await send.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<RequirementsResultDtoModel>(content, _options);
    }

    /// <summary>
    /// Compares two PC configurations.
    /// </summary>
    /// <param name="a">The first PC configuration.</param>
    /// <param name="b">The second PC configuration.</param>
    /// <returns>A list of comparison messages, or null if the request fails.</returns>
    public async Task<List<PcCompareMessage>?> ComparePcs(PcDtoModel a, PcDtoModel b)
    {
        ComparePcsRequest body = new(a, b);
        var send = await _httpClient.PostAsJsonAsync($"{ServerAddress}/pc/compare/pc", body);
        if (!send.IsSuccessStatusCode) return null;
        var content = await send.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<PcCompareMessage>>(content, _options);
    }
    
    /// <summary>
    /// Retrieves a CPU benchmark by the CPU name.
    /// </summary>
    /// <param name="name">The name of the CPU.</param>
    /// <returns>The CPU benchmark data, or null if not found or the request fails.</returns>
    public async Task<CpuBenchmarkDtoModel?> GetCpuBenchmark(string name)
    {
        var response = await _httpClient.GetAsync($"{ServerAddress}/benchmarks/cpu/byName/{name}");
        if (!response.IsSuccessStatusCode) return null;
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<CpuBenchmarkDtoModel>(content, _options);
    }
    
    /// <summary>
    /// Retrieves a GPU benchmark by the GPU name.
    /// </summary>
    /// <param name="name">The name of the GPU.</param>
    /// <returns>The GPU benchmark data, or null if not found or the request fails.</returns>
    public async Task<GpuBenchmarkDtoModel?> GetGpuBenchmark(string name)
    {
        var response = await _httpClient.GetAsync($"{ServerAddress}/benchmarks/gpu/byName/{name}");
        if (!response.IsSuccessStatusCode) return null;
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<GpuBenchmarkDtoModel>(content, _options);
    }
}