using PCFromScratch.DBModels;

namespace PCFromScratch.Repository;

public class FakeRam : IRamRepository
{
    private List<Ram> _rams = new List<Ram>
    {
        new() { Id = Guid.NewGuid(), Name = "Kingston FURY Beast Black 32GB (2x16GB) DDR5 6000", Amount = 32, Sticks = 2, Voltage = 1.35f, Generation = "DDR5", Frequency = 6000 },
        new() { Id = Guid.NewGuid(), Name = "G.Skill Trident Z5 Neo RGB 64GB (2x32GB) DDR5 6000", Amount = 64, Sticks = 2, Voltage = 1.4f, Generation = "DDR5", Frequency = 6000 },
        new() { Id = Guid.NewGuid(), Name = "Corsair Vengeance LPX 16GB (2x8GB) DDR4 3200", Amount = 16, Sticks = 2, Voltage = 1.35f, Generation = "DDR4", Frequency = 3200 },
        new() { Id = Guid.NewGuid(), Name = "Patriot Signature Line 8GB DDR4 2666", Amount = 8, Sticks = 1, Voltage = 1.2f, Generation = "DDR4", Frequency = 2666 }
    };
    public async IAsyncEnumerable<Ram> GetRams(string? generation)
    {
        foreach (var ram in _rams)
        {
            yield return ram;
        }
    }

    public async Task<Ram?> GetRam(Guid id)
    {
        return _rams.Where(d => d.Id == id).FirstOrDefault();
    }

    public Task AddRam(Ram ram)
    {
        throw new NotImplementedException();
    }

    public Task UpdateRam(Ram ram)
    {
        throw new NotImplementedException();
    }

    public Task RemoveRam(Guid id)
    {
        throw new NotImplementedException();
    }
}