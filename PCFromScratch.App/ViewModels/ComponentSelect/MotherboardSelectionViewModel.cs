using System.Collections.ObjectModel;
using PCFromScratch.App.Utils;
using PCFromScratch.DTOModels;

namespace PCFromScratch.App.ViewModels;

public class MotherboardSelectionViewModel : BaseComponentViewModel<MotherboardDtoModel>
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

    public ObservableCollection<string> RamGenerations { get; } = new();
    
    private string _selectedRamGeneration;
    public string SelectedRamGeneration
    {
        get => _selectedRamGeneration;
        set
        {
            if (_selectedRamGeneration == value) return;
            _selectedRamGeneration = value;
            OnPropertyChanged();
            LoadParts(true);
        }
    }

    public MotherboardSelectionViewModel(ServerRequests serverRequests)
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
            _allParts = await _serverRequests.GetItems<MotherboardDtoModel>("/motherboard") ?? new List<MotherboardDtoModel>();
            
            var sockets = _allParts.Select(c => c.Socket).Distinct().ToList();
            Sockets.Clear();
            Sockets.Add("Усі");
            foreach (var socket in sockets)
            {
                Sockets.Add(socket);
            }
            SelectedSocket = "Усі";

            var ramGens = _allParts.Select(c => c.RamGeneration).Distinct().ToList();
            RamGenerations.Clear();
            RamGenerations.Add("Усі");
            foreach (var gen in ramGens)
            {
                RamGenerations.Add(gen);
            }
            SelectedRamGeneration = "Усі";

            LoadParts();
        }
        finally
        {
            IsBusy = false;
        }
    }

    protected override IEnumerable<MotherboardDtoModel> ApplyFilters(IEnumerable<MotherboardDtoModel> parts)
    {
        if (SelectedSocket != "Усі")
        {
            parts = parts.Where(p => p.Socket == SelectedSocket);
        }
        if (SelectedRamGeneration != "Усі")
        {
            parts = parts.Where(p => p.RamGeneration == SelectedRamGeneration);
        }
        return base.ApplyFilters(parts);
    }
}