using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace PCFromScratch.App.ViewModels
{
    public abstract class BaseComponentViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ComponentModel> Parts { get; set; }
        protected ObservableCollection<ComponentModel> _allParts;

        private string _searchTerm;
        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                _searchTerm = value;
                UpdateList(_searchTerm);
                OnPropertyChanged();
            }
        }

        public ICommand SelectCommand { get; }

        protected BaseComponentViewModel()
        {
            Parts = new ObservableCollection<ComponentModel>();
            _allParts = new ObservableCollection<ComponentModel>();
            SelectCommand = new Command<ComponentModel>(OnSelect);
            _searchTerm = "";
        }

        protected abstract void LoadParts();

        protected void UpdateList(string searchTerm = "")
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

        private async void OnSelect(ComponentModel part)
        {
            var navigationParameter = new Dictionary<string, object>
            {
                { "SelectedPart", part },
                { "Category", GetType().Name.Replace("SelectionViewModel", "") }
            };
            await Shell.Current.GoToAsync("..", navigationParameter);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}