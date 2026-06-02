using PCFromScratch.DBModels;
using PCFromScratch.Repository;

namespace PCFromScratch.App.ViewModels;

public class RamSelectionViewModel : BaseComponentViewModel<Ram>
{
    private readonly IRamRepository _ramRepository;

    public RamSelectionViewModel(IRamRepository ramRepository)
    {
        _ramRepository = ramRepository;
        LoadParts();
    }

    protected override async void LoadParts()
    {
        _allParts.Clear();
        await foreach (var ram in _ramRepository.GetRams())
        {
            _allParts.Add(ram);
        }
        UpdateList();
    }
}