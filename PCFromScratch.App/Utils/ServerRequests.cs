using System.Net.Http.Json;
using System.Text.Json;

using PCFromScratch.Common;
using PCFromScratch.DTOModels;

namespace PCFromScratch.App.Utils;

public class ServerRequests
{
    public readonly string ServerAddress;
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _options;

    public ServerRequests()
    {
        ServerAddress = "http://192.168.0.77:5160";
        _httpClient = new HttpClient();
        _options = new JsonSerializerOptions
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
            return JsonSerializer.Deserialize<IEnumerable<T>>(content, _options);
        }
        return null;
    }

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

    public async Task<IEnumerable<Warning>?> CheckPc(PcDtoModel pc)
    {
        var send = await _httpClient.PostAsJsonAsync($"{ServerAddress}/pc/check",pc);
        if (!send.IsSuccessStatusCode) return null;
        var response = await _httpClient.GetAsync($"{ServerAddress}/pc/check");
        if (!response.IsSuccessStatusCode) return null;
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<IEnumerable<Warning>>(content, _options);
    }
    
    public async Task<(bool, Dictionary<string, string>)?> CheckRequirements(PcDtoModel pc,
        SystemRequirementsDtoModel systemRequirements)
    {
        CompareRequirementsRequest body = new (pc, systemRequirements);
        var send = await _httpClient.PostAsJsonAsync($"{ServerAddress}/pc/check", body);
        if (!send.IsSuccessStatusCode) return null;
        var response = await _httpClient.GetAsync($"{ServerAddress}/pc/check");
        if (!response.IsSuccessStatusCode) return null;
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<(bool, Dictionary<string, string>)>(content, _options);
    }

    public async Task<List<PcCompareMessage>?> ComparePcs(PcDtoModel a, PcDtoModel b)
    {
        ComparePcsRequest body = new(a, b);
        var send = await _httpClient.PostAsJsonAsync($"{ServerAddress}/pc/compare/pc", body);
        if (!send.IsSuccessStatusCode) return null;
        var response = await _httpClient.GetAsync($"{ServerAddress}/pc/compare/pc");
        if (!response.IsSuccessStatusCode) return null;
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<PcCompareMessage>>(content, _options);
    }
    
    public async Task<CpuBenchmarkDtoModel?> GetCpuBenchmark(string name)
    {
        var response = await _httpClient.GetAsync($"{ServerAddress}/benchmarks/cpu/byName/{name}");
        if (!response.IsSuccessStatusCode) return null;
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<CpuBenchmarkDtoModel>(content, _options);
    }
    
    public async Task<GpuBenchmarkDtoModel?> GetGpuBenchmark(string name)
    {
        var response = await _httpClient.GetAsync($"{ServerAddress}/benchmarks/gpu/byName/{name}");
        if (!response.IsSuccessStatusCode) return null;
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<GpuBenchmarkDtoModel>(content, _options);
    }
}