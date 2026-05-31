using PCFromScratch.DBModels;

namespace PCFromScratch.Repository;

public interface IMotherboardRepository
{
    IAsyncEnumerable<Motherboard> GetMotherboards(string? socket = null);

    Task<Motherboard?> GetMotherboard(Guid id);

    Task AddMotherboard(Motherboard motherboard);
    Task UpdateMotherboard(Motherboard motherboard);
    Task RemoveMotherboard(Guid id);
}
