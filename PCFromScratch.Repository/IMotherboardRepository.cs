using PCFromScratch.DBModels;

namespace PCFromScratch.Repository;

public interface IMotherboardRepository
{
    IAsyncEnumerable<Motherboard> GetMotherboards();

    Task<Motherboard?> GetMotherboard(Guid id);

    IAsyncEnumerable<Motherboard> GetMotherboardsBySocket(string socket);

    Task AddMotherboard(Motherboard motherboard);
    Task UpdateMotherboard(Motherboard motherboard);
    Task RemoveMotherboard(Guid id);
}
