using PCFromScratch.Common;
using PCFromScratch.DBModels;

namespace PCFromScratch.Repository;

public class FakePsu : IPsuRepository
{
    private List<Psu> _psus = new List<Psu>
    {
        new() { Id = Guid.NewGuid(), Name = "Cooler Master MWE 1050 V2 Gold", Power = 1050, Level = PsuLevel.Gold, PowerConnector = "24", FormFactor = PsuFormFactor.ATX, Modularity = PsuModularity.Modular },
        new() { Id = Guid.NewGuid(), Name = "be quiet! System Power 10 850W", Power = 850, Level = PsuLevel.Bronze, PowerConnector = "24", FormFactor = PsuFormFactor.ATX, Modularity = PsuModularity.Modular },
        new() { Id = Guid.NewGuid(), Name = "Deepcool PX1000G", Power = 1000, Level = PsuLevel.Gold, PowerConnector = "24", FormFactor = PsuFormFactor.ATX, Modularity = PsuModularity.SemiModular },
        new() { Id = Guid.NewGuid(), Name = "MSI MAG A850GL PCIE5", Power = 850, Level = PsuLevel.Gold, PowerConnector = "24", FormFactor = PsuFormFactor.ATX, Modularity = PsuModularity.NotModular }
    };
    public async IAsyncEnumerable<Psu> GetPsus(int? capacity)
    {
        foreach (var psu in _psus)
        {
            yield return psu;
        }
    }

    public async Task<Psu?> GetPsu(Guid id)
    {
        return _psus.Where(d => d.Id == id).FirstOrDefault();
    }

    public Task AddPsu(Psu psu)
    {
        throw new NotImplementedException();
    }

    public Task UpdatePsu(Psu psu)
    {
        throw new NotImplementedException();
    }

    public Task RemovePsu(Guid id)
    {
        throw new NotImplementedException();
    }
}