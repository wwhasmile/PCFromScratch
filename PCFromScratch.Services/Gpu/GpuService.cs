using PCFromScratch.Common;
using PCFromScratch.DBModels;
using PCFromScratch.DTOModels;
using PCFromScratch.Repository;

namespace PCFromScratch.Services;

public class GpuService(IGpuRepository gpuRepository) : IGpuService
{
    public async IAsyncEnumerable<GpuDtoModel> GetGpus()
    {
        await foreach (var gpu in gpuRepository.GetGpus())
            yield return new(gpu.Id, gpu.Name, gpu.Tdp, gpu.Length, gpu.ImageUrl, gpu.MinPrice, gpu.MaxPrice);
    }

    public async Task<GpuDtoModel?> GetGpu(Guid id)
    {
        var gpu = await gpuRepository.GetGpu(id);
        if (gpu is null) return null;

        return new(gpu.Id, gpu.Name, gpu.Tdp, gpu.Length, gpu.ImageUrl, gpu.MinPrice, gpu.MaxPrice);
    }

    public async IAsyncEnumerable<OfferDtoModel> GetGpuOffers(Guid id)
    {
        var gpu = await gpuRepository.GetGpu(id);
        if (gpu is not null)
            foreach (var offer in gpu.Offers)
                yield return new(offer.ShopName, offer.Price, offer.City);
    }
}