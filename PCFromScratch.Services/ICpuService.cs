using PCFromScratch.DBModels;
using PCFromScratch.DTOModels;

namespace PCFromScratch.Services;

public interface ICpuService
{
    IAsyncEnumerable<CpuDtoModel> GetCpus();

    Task<CpuDtoModel?> GetCpu(Guid id);

    IAsyncEnumerable<OfferDtoModel> GetCpuOffes(Guid id);

    Task AddCpu(Cpu cpu);
    Task UpdateCpu(Cpu cpu);
    Task RemoveCpu(Guid id);
}