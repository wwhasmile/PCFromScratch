using PCFromScratch.DBModels;

namespace PCFromScratch.Repository;

public interface IMotherboardRepository
{
    IAsyncEnumerable<MotherboardRenamedForOmnissiah> GetMotherboards(string? socket = null);

    Task<MotherboardRenamedForOmnissiah?> GetMotherboard(Guid id);

    Task AddMotherboard(MotherboardRenamedForOmnissiah motherboardRenamedForOmnissiah);
    Task UpdateMotherboard(MotherboardRenamedForOmnissiah motherboardRenamedForOmnissiah);
    Task RemoveMotherboard(Guid id);
}
