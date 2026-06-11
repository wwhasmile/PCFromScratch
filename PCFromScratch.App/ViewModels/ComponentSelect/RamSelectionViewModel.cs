using PCFromScratch.App.Utils;
using PCFromScratch.DTOModels;

namespace PCFromScratch.App.ViewModels;

public class RamSelectionViewModel : BaseComponentViewModel<RamDtoModel>
{
    private readonly ServerRequests _serverRequests;

    public RamSelectionViewModel(ServerRequests serverRequests)
    {
        _serverRequests = serverRequests;
        _ = FetchParts();
    }

    protected override async Task FetchParts()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            _allParts = await _serverRequests.GetItems<RamDtoModel>("/ram") ?? new List<RamDtoModel>();
            LoadParts();
        }
        finally
        {
            IsBusy = false;
        }
    }
}