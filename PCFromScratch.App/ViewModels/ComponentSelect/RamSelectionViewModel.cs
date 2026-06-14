using System.Collections.ObjectModel;
using PCFromScratch.App.Utils;
using PCFromScratch.DTOModels;

namespace PCFromScratch.App.ViewModels;

public class RamSelectionViewModel : BaseComponentViewModel<RamDtoModel>
{
    private readonly ServerRequests _serverRequests;

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
            
            var ramGens = _allParts.Select(c => c.Generation).Distinct().ToList();
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

    protected override IEnumerable<RamDtoModel> ApplyFilters(IEnumerable<RamDtoModel> parts)
    {
        if (SelectedRamGeneration != "Усі")
        {
            parts = parts.Where(p => p.Generation == SelectedRamGeneration);
        }
        return base.ApplyFilters(parts);
    }
}