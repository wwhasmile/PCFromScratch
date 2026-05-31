using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PCFromScratch.App.ViewModels;

namespace PCFromScratch.App.Pages;

public partial class PcConstructorPage
{
    public PcConstructorPage()
    {
        InitializeComponent();
        BindingContext = new PcConstructorViewModel();
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