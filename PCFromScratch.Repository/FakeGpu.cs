using PCFromScratch.DBModels;

namespace PCFromScratch.Repository;

public class FakeGpu : IGpuRepository
{
    public async IAsyncEnumerable<Gpu> GetGpus()
    {
        var gpus = new List<Gpu>
        {
            new Gpu { Id = Guid.NewGuid(), Name = "Gigabyte Radeon RX 9060 XT GAMING OC 16G", Tdp = 450 },
            new Gpu { Id = Guid.NewGuid(), Name = "Gigabyte GeForce RTX 5080 GAMING OC 16G", Tdp = 320 },
            new Gpu { Id = Guid.NewGuid(), Name = "MSI GeForce RTX 5070 12G GAMING TRIO OC", Tdp = 355 },
            new Gpu { Id = Guid.NewGuid(), Name = "Sapphire Radeon RX 9060 XT PULSE 16GB", Tdp = 170 }
        };

        foreach (var gpu in gpus)
        {
            yield return gpu;
        }
    }

    public Task<Gpu?> GetGpu(Guid id)
    {
        throw new NotImplementedException();
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