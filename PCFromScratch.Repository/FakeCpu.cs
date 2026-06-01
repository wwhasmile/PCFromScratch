using PCFromScratch.Common;
using PCFromScratch.DBModels;

namespace PCFromScratch.Repository;

public class FakeCpu : ICpuRepository
{
    public async IAsyncEnumerable<Cpu> GetCpus()
    {
        var cpus = new List<Cpu>
        {
            new Cpu {Id = Guid.NewGuid(), Name = "Intel Core Ultra 7 270K Plus", Socket = "1151", MaxRam = 128, RamGen = "DDR5", Tdp = 165, Packing = CpuPacking.Box },
            new Cpu {Id = Guid.NewGuid(), Name = "AMD Ryzen 5 Matisse 3600", Socket = "AM5", MaxRam = 256, RamGen = "DDR5", Tdp = 165, Packing = CpuPacking.OEM },
            new Cpu {Id = Guid.NewGuid(), Name = "Intel Core i9 Raptor Lake Refresh i9-14900K", Socket = "1151", MaxRam = 256, RamGen = "DDR5", Tdp = 165, Packing = CpuPacking.OEM },
            new Cpu {Id = Guid.NewGuid(), Name = "AMD Ryzen 5 Summit Ridge 1600 OEM 1", Socket = "AM4", MaxRam = 128, RamGen = "DDR5", Tdp = 165, Packing = CpuPacking.Box }
        };
        foreach (var cpu in cpus)
        {
            yield return cpu;
        }
    }

    public Task<Cpu?> GetCpu(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task AddCpu(Cpu cpu)
    {
        throw new NotImplementedException();
    }

    public Task UpdateCpu(Cpu cpu)
    {
        throw new NotImplementedException();
    }

    public Task RemoveCpu(Guid id)
    {
        throw new NotImplementedException();
    }
}