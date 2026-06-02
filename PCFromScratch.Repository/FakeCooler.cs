using PCFromScratch.Common;
using PCFromScratch.DBModels;

namespace PCFromScratch.Repository;

public class FakeCooler : ICoolerRepository
{
    private List<Cooler> _coolers = new List<Cooler>
    {
        new() { Id = Guid.NewGuid(), Name = "ID-Cooling SE-224-XTS Black", Tdp = 220, Type = CoolerType.ActiveCooler, FanCount = 1, Speed = 1500, Height = 151 },
        new() { Id = Guid.NewGuid(), Name = "be quiet! Dark Rock Pro 5", Tdp = 270, Type = CoolerType.ActiveCooler, FanCount = 2, Speed = 1700, Height = 168 },
        new() { Id = Guid.NewGuid(), Name = "Deepcool LS720", Tdp = 300, Type = CoolerType.WaterCooler, FanCount = 3, Speed = 2250, Height = 52 },
        new() { Id = Guid.NewGuid(), Name = "MSI MAG CoreLiquid E360", Tdp = 280, Type = CoolerType.WaterCooler, FanCount = 3, Speed = 1800, Height = 52 }
    };
    public async IAsyncEnumerable<Cooler> GetCoolers(string? socket)
    {
        foreach (var cooler in _coolers)
        {
            yield return cooler;
        }
    }

    public async Task<Cooler?> GetCooler(Guid id)
    {
        return _coolers.Where(c => c.Id == id).FirstOrDefault();
    }

    public Task AddCooler(Cooler cooler)
    {
        throw new NotImplementedException();
    }

    public Task UpdateCooler(Cooler cooler)
    {
        throw new NotImplementedException();
    }

    public Task RemoveCooler(Guid id)
    {
        throw new NotImplementedException();
    }
}