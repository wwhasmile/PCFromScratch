using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PCFromScratch.App.Pages;

public class Part (Guid id, string name, IEnumerable<Offer> offers) : INotifyPropertyChanged
{
    public Guid Id { get; set; } = id;
    public string Name { get; set; } = name;
    public ObservableCollection<Offer> Offers { get; set; } = new (offers);
    
    public Offer? SelectedOffer
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class Offer(string shop, string link, decimal price)
{
    public string Shop { get; set; } = shop;
    public string Link { get; set; } = link;
    public decimal Price { get; set; } = price;
}
