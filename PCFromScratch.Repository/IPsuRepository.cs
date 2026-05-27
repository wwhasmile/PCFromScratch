using PCFromScratch.DBModels;

namespace PCFromScratch.Repository;

public interface IPsuRepository
{
    IAsyncEnumerable<Psu> GetPsus();

    Task<Psu> GetPsu(Guid id);

    IAsyncEnumerable<Psu> GetPsusFromPower(int power);

    Task AddPsu(Psu psu);
    Task UpdatePsu(Psu psu);
    Task RemovePsu(Guid id);
}
