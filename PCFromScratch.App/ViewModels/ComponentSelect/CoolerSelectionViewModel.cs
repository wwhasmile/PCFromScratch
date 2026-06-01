using PCFromScratch.Repository;

namespace PCFromScratch.App.ViewModels
{
    public class CoolerSelectionViewModel : BaseComponentViewModel
    {
        private readonly ICoolerRepository _coolerRepository;

        public CoolerSelectionViewModel(ICoolerRepository coolerRepository)
        {
            _coolerRepository = coolerRepository;
            LoadParts();
        }

        protected override async void LoadParts()
        {
            _allParts.Clear();
            await foreach (var cooler in _coolerRepository.GetCoolers())
            {
                _allParts.Add(new ComponentModel { Id = cooler.Id, Name = cooler.Name });
            }
            UpdateList();
        }
    }
}