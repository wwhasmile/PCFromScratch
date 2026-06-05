using PCFromScratch.Common;
using PCFromScratch.DBModels;

namespace PCFromScratch.Repository;

public class FakeMotherboard : IMotherboardRepository
{
    private List<MotherboardRenamedForOmnissiah> _motherboards = new List<MotherboardRenamedForOmnissiah>
    {
        new() { Id = Guid.NewGuid(), Link = "link", Name = "Asus ROG STRIX Z790-E GAMING WIFI II", Socket = "1151", FormFactor = MotherboardFormFactor.ATX, Chipset = "Z790", RamGeneration = "DDR5", RamSlots = 4, RamFrequency = 8000},
        new() { Id = Guid.NewGuid(), Link = "link", Name = "MSI MAG B650 TOMAHAWK WIFI", Socket = "AM5", FormFactor = MotherboardFormFactor.ATX, Chipset = "B650", RamGeneration = "DDR5", RamSlots = 4, RamFrequency = 7600},
        new() { Id = Guid.NewGuid(), Link = "link", Name = "Gigabyte B550M AORUS ELITE AX", Socket = "AM4", FormFactor = MotherboardFormFactor.MicroATX, Chipset = "B550", RamGeneration = "DDR4", RamSlots = 4, RamFrequency = 4733},
        new() { Id = Guid.NewGuid(), Link = "link", Name = "Asrock H610M-HVS/M.2", Socket = "1151", FormFactor = MotherboardFormFactor.MicroATX, Chipset = "H610", RamGeneration = "DDR4", RamSlots = 2, RamFrequency = 3200}
    };
    public async IAsyncEnumerable<MotherboardRenamedForOmnissiah> GetMotherboards(string? socket)
    {
        foreach (var motherboard in _motherboards)
        {
            yield return motherboard;
        }
    }

    public async Task<MotherboardRenamedForOmnissiah?> GetMotherboard(Guid id)
    {
        return _motherboards.Where(m => m.Id == id).FirstOrDefault();
    }

    public Task AddMotherboard(MotherboardRenamedForOmnissiah motherboardRenamedForOmnissiah)
    {
        throw new NotImplementedException();
    }

    public Task UpdateMotherboard(MotherboardRenamedForOmnissiah motherboardRenamedForOmnissiah)
    {
        throw new NotImplementedException();
    }

    public Task RemoveMotherboard(Guid id)
    {
        throw new NotImplementedException();
    }
}