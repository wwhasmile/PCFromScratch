using System.Collections.ObjectModel;
using PCFromScratch.App.Utils;
using PCFromScratch.DTOModels;

namespace PCFromScratch.App.ViewModels;

public class CpuSelectionViewModel : BaseComponentViewModel<CpuDtoModel>
{
    private readonly ServerRequests _serverRequests;

    public ObservableCollection<string> Sockets { get; } = new();
    
    private string _selectedSocket;
    public string SelectedSocket
    {
        get => _selectedSocket;
        set
        {
            if (_selectedSocket == value) return;
            _selectedSocket = value;
            OnPropertyChanged();
            LoadParts(true);
        }
    }

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
            
            var sockets = _allParts.Select(c => c.Socket).Distinct().ToList();
            Sockets.Clear();
            Sockets.Add("Усі");
            foreach (var socket in sockets)
            {
                Sockets.Add(socket);
            }
            SelectedSocket = "Усі";

            LoadParts();
        }
        finally
        {
            IsBusy = false;
        }
    }

    protected override IEnumerable<CpuDtoModel> ApplyFilters(IEnumerable<CpuDtoModel> parts)
    {
        if (SelectedSocket != "Усі")
        {
            parts = parts.Where(p => p.Socket == SelectedSocket);
        }
        return base.ApplyFilters(parts);
    }
}