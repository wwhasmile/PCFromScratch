using PCFromScratch.DBModels;
using PCFromScratch.Storage;

namespace PCFromScratch.Repository;

public class StorageCpuRepository(IStorageContext storageContext) : ICpuRepository
{
    private readonly IStorageContext _storageContext = storageContext;

    public IAsyncEnumerable<Cpu> GetCpus(string? socket) => _storageContext.GetCpus(socket);

    public Task<Cpu?> GetCpu(Guid id) => _storageContext.GetCpu(id);

    public Task AddCpu(Cpu cpu) => _storageContext.AddCpu(cpu);

    public Task UpdateCpu(Cpu cpu) => _storageContext.UpdateCpu(cpu);

    public Task RemoveCpu(Guid id) => _storageContext.RemoveCpu(id);
}