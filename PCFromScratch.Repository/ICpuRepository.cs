using PCFromScratch.DBModels;

namespace PCFromScratch.Repository;

public interface ICpuRepository
{
    IAsyncEnumerable<Cpu> GetCpus();

    Task<Cpu?> GetCpu(Guid id);

    Task AddCpu(Cpu cpu);
    Task UpdateCpu(Cpu cpu);
    Task RemoveCpu(Guid id);
}
