using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PCFromScratch.App.Pages;

public class Part : INotifyPropertyChanged
{
    public string Name { get; set; }
    public ObservableCollection<Offer> Offers { get; set; } = new();

    private Offer _selectedOffer;
    public Offer SelectedOffer
    {
        get => _selectedOffer;
        set
        {
            if (_selectedOffer != value)
            {
                _selectedOffer = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class Offer
{
    public string Shop { get; set; }
    public decimal Price { get; set; }
}
