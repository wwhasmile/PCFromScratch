using PCFromScratch.App.Utils;
using PCFromScratch.DTOModels;

namespace PCFromScratch.App.ViewModels;

public class CpuSelectionViewModel : BaseComponentViewModel<CpuDtoModel>
{
    private readonly ServerRequests _serverRequests;

    public CpuSelectionViewModel(ServerRequests serverRequests)
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
            _allParts = await _serverRequests.GetItems<CpuDtoModel>("/cpu") ?? new List<CpuDtoModel>();
            LoadParts();
        }
        finally
        {
            IsBusy = false;
        }
    }
}