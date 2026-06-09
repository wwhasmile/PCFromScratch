using PCFromScratch.Common;
using PCFromScratch.DBModels;
using PCFromScratch.DTOModels;
using PCFromScratch.Repository;

namespace PCFromScratch.Services;

public class CpuService(ICpuRepository cpuRepository) : ICpuService
{
    private readonly ICpuRepository _cpuRepository = cpuRepository;

    public async IAsyncEnumerable<CpuDtoModel> GetCpus()
    {
        await foreach (var cpu in _cpuRepository.GetCpus())
        {
            yield return new(cpu.Id, cpu.Name, cpu.Socket, cpu.Tdp, cpu.RamGen, cpu.RamFrequency,
                cpu.Packing.GetDisplayName(), cpu.ImageUrl, cpu.MinPrice, cpu.MaxPrice);
        }
    }

    public async Task<CpuDtoModel?> GetCpu(Guid id)
    {
        var cpu = await _cpuRepository.GetCpu(id);
        if (cpu is null) return null;

        return new(cpu.Id, cpu.Name, cpu.Socket, cpu.Tdp, cpu.RamGen, cpu.RamFrequency,
            cpu.Packing.GetDisplayName(), cpu.ImageUrl, cpu.MinPrice, cpu.MaxPrice);
    }

    public async IAsyncEnumerable<OfferDtoModel> GetCpuOffes(Guid id)
    {
        var cpu = await _cpuRepository.GetCpu(id);
        if (cpu is not null)
        {
            foreach (var offer in cpu.Offers)
                yield return new(offer.ShopName, offer.Price, offer.City);
        }
    }

    public Task AddCpu(Cpu cpu) => _cpuRepository.AddCpu(cpu);

    public Task UpdateCpu(Cpu cpu) => _cpuRepository.UpdateCpu(cpu);

    public Task RemoveCpu(Guid id) => _cpuRepository.RemoveCpu(id);
}