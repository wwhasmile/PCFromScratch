using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PCFromScratch.App.ViewModels;

namespace PCFromScratch.App.Pages;

[QueryProperty(nameof(SelectedPart), "SelectedPart")]
[QueryProperty(nameof(Category), "Category")]
public partial class PcConstructorPage
{
    public ComponentModel SelectedPart { get; set; }
    public string Category { get; set; }

    public PcConstructorPage(PcConstructorViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (SelectedPart != null && !string.IsNullOrEmpty(Category))
        {
            var viewModel = (PcConstructorViewModel)BindingContext;
            viewModel.UpdateSelectedComponent(Category, SelectedPart);

            // Clear properties to prevent re-processing
            SelectedPart = null;
            Category = null;
        }
    }
}

public abstract class BaseComponentCategory (string name) : INotifyPropertyChanged
{
    public string Name { get; set; } = name;
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class SingleComponentCategory (string name): BaseComponentCategory (name)
{
    public Part? SelectedPart
    {
        get => field;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ButtonText));
                SelectedOffer = field?.Offers.FirstOrDefault();
            }
        }
    }
    
    public Offer? SelectedOffer
    {
        get => field;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }
    public string ButtonText => SelectedPart == null ? "Обрати" : "Змінити";
}

public class MultiComponentCategory (string name): BaseComponentCategory (name)
{
    public ObservableCollection<Part> SelectedParts { get; } = new();
}