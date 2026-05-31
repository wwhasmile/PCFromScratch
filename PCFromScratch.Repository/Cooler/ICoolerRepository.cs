using PCFromScratch.DBModels;

namespace PCFromScratch.Repository;

public interface ICoolerRepository
{
    IAsyncEnumerable<Cooler> GetCoolers(string? socket = null);

    Task<Cooler?> GetCooler(Guid id);

    Task AddCooler(Cooler cooler);
    Task UpdateCooler(Cooler cooler);
    Task RemoveCooler(Guid id);
}
