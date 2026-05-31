using PCFromScratch.DBModels;
using PCFromScratch.Storage;

namespace PCFromScratch.Repository;

public class StoragePsuRepository(IStorageContext storageContext) : IPsuRepository
{
    private readonly IStorageContext _storageContext = storageContext;

    public IAsyncEnumerable<Psu> GetPsus(int? minPower = null) => _storageContext.GetPsus(minPower);

    public Task<Psu?> GetPsu(Guid id) => _storageContext.GetPsu(id);

    public Task AddPsu(Psu psu) => _storageContext.AddPsu(psu);

    public Task UpdatePsu(Psu psu) => _storageContext.UpdatePsu(psu);

    public Task RemovePsu(Guid id) => _storageContext.RemovePsu(id);
}