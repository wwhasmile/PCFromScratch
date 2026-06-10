using PCFromScratch.Common;
using PCFromScratch.DBModels;
using PCFromScratch.DTOModels;
using PCFromScratch.Repository;

namespace PCFromScratch.Services;

public class CpuService(ICpuRepository cpuRepository) : ICpuService
{
    private readonly ICpuRepository _cpuRepository = cpuRepository;

    public async IAsyncEnumerable<CpuDtoModel> GetCpus(string? socket = null)
    {
        await foreach (var cpu in _cpuRepository.GetCpus(socket))
        {
            yield return new(cpu.Id, cpu.Name, cpu.Socket, cpu.Tdp, cpu.RamGen, cpu.RamFrequency, cpu.Packing,
                cpu.ImageUrl, cpu.MinPrice, cpu.MaxPrice);
        }
    }

    public async Task<CpuDtoModel?> GetCpu(Guid id)
    {
        var cpu = await _cpuRepository.GetCpu(id);
        if (cpu is null) return null;

        return new(cpu.Id, cpu.Name, cpu.Socket, cpu.Tdp, cpu.RamGen, cpu.RamFrequency, cpu.Packing,
            cpu.ImageUrl, cpu.MinPrice, cpu.MaxPrice);
    }

    public async IAsyncEnumerable<OfferDtoModel> GetCpuOffers(Guid id)
    {
        var cpu = await _cpuRepository.GetCpu(id);
        if (cpu is not null)
        {
            foreach (var offer in cpu.Offers)
                yield return new(offer.ShopName, offer.Price, offer.City);
        }
    }
}