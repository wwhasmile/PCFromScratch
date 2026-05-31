using PCFromScratch.DBModels;

namespace PCFromScratch.Repository;

public interface IRamRepository
{
    IAsyncEnumerable<Ram> GetRams(string? generation = null);

    Task<Ram?> GetRam(Guid id);

    Task AddRam(Ram ram);
    Task UpdateRam(Ram ram);
    Task RemoveRam(Guid id);
}
