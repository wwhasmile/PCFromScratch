using System.Collections.ObjectModel;

namespace PCFromScratch.App.Pages;

[QueryProperty(nameof(Category), "Category")]
public partial class ComponentSelectionPage : ContentPage
{
    private string _category;
    public string Category
    {
        get => _category;
        set
        {
            _category = value;
            LoadParts(_category);
        }
    }

    public ObservableCollection<Part> Parts { get; set; }
    private ObservableCollection<Part> _allParts;

    public ComponentSelectionPage()
    {
        InitializeComponent();
        Parts = new ObservableCollection<Part>();
        _allParts = new ObservableCollection<Part>();
        PartCollectionView.ItemsSource = Parts;
    }

    private void LoadParts(string category)
    {
        // Simulate loading part models based on the category
        _allParts.Clear();
        for (int i = 1; i <= 10; i++)
        {
            var part = new Part
            {
                Name = $"{category} Модель {i}"
            };

            // Add simulated offers
            part.Offers.Add(new Offer { Shop = "Rozetka", Price = i * 1000 + 50 });
            part.Offers.Add(new Offer { Shop = "Comfy", Price = i * 1000 + 100 });
            part.Offers.Add(new Offer { Shop = "Moyo", Price = i * 1000 + 20 });

            _allParts.Add(part);
        }

        UpdateList();
    }

    private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateList(e.NewTextValue);
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

    private async void SelectButton_Clicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var part = (Part)button.CommandParameter;

        // In a real app, you'd use messaging center, viewmodels, or route parameters to pass this back
        // For now, let's just go back. We need to implement passing data back.
        // For example, passing ID: await Shell.Current.GoToAsync($"..?SelectedPartId={part.Id}");
        await Shell.Current.GoToAsync("..");
    }
}
