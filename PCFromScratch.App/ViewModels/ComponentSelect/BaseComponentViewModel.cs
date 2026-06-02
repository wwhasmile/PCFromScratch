using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

using PCFromScratch.Common;
using PCFromScratch.DBModels;

namespace PCFromScratch.App.ViewModels
{
    public abstract class BaseComponentViewModel<T> : INotifyPropertyChanged where T: class
    {
        public ObservableCollection<T> Parts { get; set; }
        protected ObservableCollection<T> _allParts;

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
            Parts = new ObservableCollection<T>();
            _allParts = new ObservableCollection<T>();
            SelectCommand = new Command<T>(OnSelect);
            _searchTerm = "";
        }

        protected abstract void LoadParts();

        protected void UpdateList(string searchTerm = "")
        {
            Parts.Clear();
            var filteredParts = string.IsNullOrWhiteSpace(searchTerm)
                ? _allParts
                : _allParts.Where(p => ((dynamic)p).Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));

            foreach (var part in filteredParts)
            {
                Parts.Add(part);
            }
        }

        private async void OnSelect(T part)
        {
            var navigationParameter = new Dictionary<string, object>
            {
                { "SelectedPartId", ((dynamic)part).Id },
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