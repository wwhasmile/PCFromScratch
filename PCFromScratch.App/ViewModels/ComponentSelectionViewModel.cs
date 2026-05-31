using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using PCFromScratch.App.Pages;

namespace PCFromScratch.App.ViewModels;

public class ComponentSelectionViewModel : INotifyPropertyChanged
{
    public string Category
    {
        get => field;
        set
        {
            field = value;
            LoadParts(field);
            OnPropertyChanged();
        }
    }

    public ObservableCollection<Part> Parts { get; set; }
    private ObservableCollection<Part> _allParts;
    
    public string SearchTerm
    {
        get => field;
        set
        {
            field = value;
            UpdateList(field);
            OnPropertyChanged();
        }
    }

    public ICommand SelectCommand { get; }

    public ComponentSelectionViewModel()
    {
        Parts = new ObservableCollection<Part>();
        _allParts = new ObservableCollection<Part>();
        SelectCommand = new Command<Part>(OnSelect);
        SearchTerm = "";
    }

    private void LoadParts(string category)
    {
        // Simulate loading part models based on the category
        _allParts.Clear();
        for (int i = 1; i <= 10; i++)
        {
            List<Offer> offers = new List<Offer>
            {
                new ("Rozetka", 500), new ("Comfy", 550), new ("Moyo", 600)
            };
            var part = new Part($"{category} Модель {i}", offers);

            _allParts.Add(part);
        }

        UpdateList();
    }

    private void UpdateList(string searchTerm = "")
    {
        Parts.Clear();
        var filteredParts = string.IsNullOrWhiteSpace(searchTerm)
            ? _allParts
            : _allParts.Where(p => p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));

        foreach (var part in filteredParts)
        {
            Parts.Add(part);
        }
    }

    private async void OnSelect(Part part)
    {
        // In a real app, you'd use messaging center, viewmodels, or route parameters to pass this back
        // For now, let's just go back. We need to implement passing data back.
        // For example, passing ID: await Shell.Current.GoToAsync($"..?SelectedPartId={part.Id}");
        await Shell.Current.GoToAsync("..");
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}