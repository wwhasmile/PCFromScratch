using PCFromScratch.DBModels;

namespace PCFromScratch.Repository;

public interface IPsuRepository
{
    IAsyncEnumerable<Psu> GetPsus(int? minPower = null);

    Task<Psu> GetPsu(Guid id);

    Task AddPsu(Psu psu);
    Task UpdatePsu(Psu psu);
    Task RemovePsu(Guid id);
}
