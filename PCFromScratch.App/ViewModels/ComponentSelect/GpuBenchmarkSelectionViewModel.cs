using PCFromScratch.App.Utils;
using PCFromScratch.DTOModels;

namespace PCFromScratch.App.ViewModels;

public class GpuBenchmarkSelectionViewModel : BaseComponentViewModel<GpuBenchmarkDtoModel>
{
    private readonly ServerRequests _serverRequests;

    public GpuBenchmarkSelectionViewModel(ServerRequests serverRequests)
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
            _allParts = await _serverRequests.GetItems<GpuBenchmarkDtoModel>("/benchmarks/gpu") ?? new List<GpuBenchmarkDtoModel>();
            LoadParts();
        }
        finally
        {
            IsBusy = false;
        }
    }
}