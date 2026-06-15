using PCFromScratch.App.Utils;
using PCFromScratch.DTOModels;

namespace PCFromScratch.App.ViewModels;

public class GpuSelectionViewModel : BaseComponentViewModel<GpuDtoModel>
{
    private readonly ServerRequests _serverRequests;

    public GpuSelectionViewModel(ServerRequests serverRequests)
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
            _allParts = await _serverRequests.GetItems<GpuDtoModel>("/gpu") ?? new List<GpuDtoModel>();
            LoadParts();
        }
        finally
        {
            IsBusy = false;
        }
    }
}