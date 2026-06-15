using PCFromScratch.App.Utils;
using PCFromScratch.DTOModels;

namespace PCFromScratch.App.ViewModels;

public class CpuBenchmarkSelectionViewModel : BaseComponentViewModel<CpuBenchmarkDtoModel>
{
    private readonly ServerRequests _serverRequests;

    public CpuBenchmarkSelectionViewModel(ServerRequests serverRequests)
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
            _allParts = await _serverRequests.GetItems<CpuBenchmarkDtoModel>("/benchmarks/cpu") ?? new List<CpuBenchmarkDtoModel>();
            LoadParts();
        }
        finally
        {
            IsBusy = false;
        }
    }
}