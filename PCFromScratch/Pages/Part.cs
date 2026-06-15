using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using PCFromScratch.DTOModels;

namespace PCFromScratch.App.Pages;

public class Part (Guid id, string name, string? image, string link, IEnumerable<OfferDtoModel> offers) : INotifyPropertyChanged
{
    public Guid Id { get; set; } = id;
    public string Name { get; set; } = name;
    public string? Image { get; set; } = image;
    public string Link { get; set; } = link;
    public ObservableCollection<OfferDtoModel> Offers { get; set; } = new (offers);
    
    public OfferDtoModel? SelectedOffer
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