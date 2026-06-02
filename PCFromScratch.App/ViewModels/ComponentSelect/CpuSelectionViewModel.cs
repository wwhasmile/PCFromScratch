using PCFromScratch.DBModels;
using PCFromScratch.Repository;

namespace PCFromScratch.App.ViewModels;

public class CpuSelectionViewModel : BaseComponentViewModel<Cpu>
{
    private readonly ICpuRepository _cpuRepository;

    public CpuSelectionViewModel(ICpuRepository cpuRepository)
    {
        _cpuRepository = cpuRepository;
        LoadParts();
    }

    protected override async void LoadParts()
    {
        _allParts.Clear();
        await foreach (var cpu in _cpuRepository.GetCpus())
        {
            _allParts.Add(cpu);
        }
        UpdateList();
    }
}