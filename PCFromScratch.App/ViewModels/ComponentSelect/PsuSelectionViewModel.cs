using PCFromScratch.DBModels;
using PCFromScratch.Repository;

namespace PCFromScratch.App.ViewModels
{
    public class PsuSelectionViewModel : BaseComponentViewModel<Psu>
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
                _allParts.Add(psu);
            }
            UpdateList();
        }
    }
}