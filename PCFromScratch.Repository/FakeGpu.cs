using PCFromScratch.DBModels;

namespace PCFromScratch.Repository;

public class FakeGpu : IGpuRepository
{
    private List<Gpu> _gpus = new List<Gpu>
    {
        new Gpu { Id = Guid.NewGuid(), Link = "link", Name = "Gigabyte Radeon RX 9060 XT GAMING OC 16G", Tdp = 450 },
        new Gpu { Id = Guid.NewGuid(), Link = "link", Name = "Gigabyte GeForce RTX 5080 GAMING OC 16G", Tdp = 320 },
        new Gpu { Id = Guid.NewGuid(), Link = "link", Name = "MSI GeForce RTX 5070 12G GAMING TRIO OC", Tdp = 355 },
        new Gpu { Id = Guid.NewGuid(), Link = "link", Name = "Sapphire Radeon RX 9060 XT PULSE 16GB", Tdp = 170 }
    };
    
    public async IAsyncEnumerable<Gpu> GetGpus()
    {
        foreach (var gpu in _gpus)
        {
            yield return gpu;
        }
    }

    public async Task<Gpu?> GetGpu(Guid id)
    {
        return _gpus.Where(c => c.Id == id).FirstOrDefault();
    }

    public Task AddGpu(Gpu gpu)
    {
        throw new NotImplementedException();
    }

    public Task UpdateGpu(Gpu gpu)
    {
        throw new NotImplementedException();
    }

    public Task RemoveGpu(Guid id)
    {
        throw new NotImplementedException();
    }
}