using PCFromScratch.Repository;

namespace PCFromScratch.App.ViewModels
{
    public class PsuSelectionViewModel : BaseComponentViewModel
    {
        private readonly IPsuRepository _psuRepository;

        public PsuSelectionViewModel(IPsuRepository psuRepository)
        {
            _psuRepository = psuRepository;
            LoadParts();
        }

        protected override async void LoadParts()
        {
            _allParts.Clear();
            await foreach (var psu in _psuRepository.GetPsus())
            {
                _allParts.Add(new ComponentModel { Id = psu.Id, Name = psu.Name });
            }
            UpdateList();
        }
    }
}