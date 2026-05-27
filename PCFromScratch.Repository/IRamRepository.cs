using PCFromScratch.DBModels;

namespace PCFromScratch.Repository;

public interface IRamRepository
{
    IAsyncEnumerable<Ram> GetRams();

    Task<Ram> GetRam(Guid id);

    IAsyncEnumerable<Ram> GetRamsByGeneration(string generation);

    Task AddRam(Ram ram);
    Task UpdateRam(Ram ram);
    Task RemoveRam(Guid id);
}
