using PCFromScratch.DBModels;

namespace PCFromScratch.Repository;

public class FakeInternalDrive : IInternalDriveRepository
{
    private List<InternalDrive> _drives = new List<InternalDrive>
    {
        new() { Id = Guid.NewGuid(), Name = "Kingston KC3000 1TB M.2 2280 NVMe PCIe 4.0", Capacity = 1000, Type = "SSD", Format = "M.2", Port = "PCIe 4.0" },
        new() { Id = Guid.NewGuid(), Name = "Samsung 990 PRO 2TB M.2 2280 NVMe PCIe 4.0", Capacity = 2000, Type = "SSD", Format = "M.2", Port = "PCIe 4.0" },
        new() { Id = Guid.NewGuid(), Name = "Crucial BX500 500GB 2.5 SATAIII", Capacity = 500, Type = "SSD", Format = "2.5", Port = "SATA III" },
        new() { Id = Guid.NewGuid(), Name = "WD Blue 1TB 7200rpm 64MB 3.5 SATA III", Capacity = 1000, Type = "HDD", Format = "3.5", Port = "SATA III" }
    };
    public async IAsyncEnumerable<InternalDrive> GetInternalDrives(string? type, int? capacity)
    {
        foreach (var drive in _drives)
        {
            yield return drive;
        }
    }

    public async Task<InternalDrive?> GetInternalDrive(Guid id)
    {
        return _drives.Where(d => d.Id == id).FirstOrDefault();
    }

    public Task AddInternalDrive(InternalDrive internalDrive)
    {
        throw new NotImplementedException();
    }

    public Task UpdateInternalDrive(InternalDrive internalDrive)
    {
        throw new NotImplementedException();
    }

    public Task RemoveInternalDrive(Guid id)
    {
        throw new NotImplementedException();
    }
}