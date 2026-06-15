using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace PCFromScratch.App.ViewModels
{
    [QueryProperty(nameof(PcIndex), "PcIndex")]
    public abstract class BaseComponentViewModel<T> : INotifyPropertyChanged
    {
        public int PcIndex { get; set; }
        public ObservableCollection<T> Parts { get; }
        protected IEnumerable<T> _allParts;

        private string _searchTerm;
        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                if (_searchTerm == value) return;
                _searchTerm = value;
                OnPropertyChanged();
                LoadParts(true);
            }
        }

        public ICommand SelectCommand { get; }
        public ICommand LoadMorePartsCommand { get; }

        protected int PageSize = 20;
        protected int _currentPage;
        
        public bool IsBusy
        {
            get => field;
            set
            {
                if (field == value) return;
                field = value;
                OnPropertyChanged();
            }
        }

        protected BaseComponentViewModel()
        {
            Parts = new ObservableCollection<T>();
            _allParts = new List<T>();
            SelectCommand = new Command<T>(OnSelect);
            LoadMorePartsCommand = new Command(() => LoadParts());
            _searchTerm = "";
        }

        protected virtual IEnumerable<T> ApplyFilters(IEnumerable<T> parts)
        {
            return parts;
        }

        protected void LoadParts(bool fromSearch = false)
        {
            try
            {
                IsBusy = true;

                if (fromSearch)
                {
                    Parts.Clear();
                    _currentPage = 0;
                }

                var filteredParts = string.IsNullOrWhiteSpace(SearchTerm)
                    ? _allParts
                    : _allParts.Where(p => ((dynamic)p!).Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));

                filteredParts = ApplyFilters(filteredParts);

                var pagedParts = filteredParts.Skip(_currentPage * PageSize).Take(PageSize).ToList();

                foreach (var part in pagedParts)
                {
                    Parts.Add(part);
                }

                if (pagedParts.Any())
                {
                    _currentPage++;
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        protected abstract Task FetchParts();

        private async void OnSelect(T part)
        {
            var navigationParameter = new Dictionary<string, object>
            {
                { "SelectedPartId", ((dynamic)part!).Id },
                { "Category", GetType().Name.Replace("SelectionViewModel", "") },
                { "PcIndex", PcIndex }
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