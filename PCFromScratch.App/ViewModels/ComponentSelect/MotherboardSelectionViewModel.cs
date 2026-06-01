using PCFromScratch.Repository;

namespace PCFromScratch.App.ViewModels
{
    public class MotherboardSelectionViewModel : BaseComponentViewModel
    {
        private readonly IMotherboardRepository _motherboardRepository;

        public MotherboardSelectionViewModel(IMotherboardRepository motherboardRepository)
        {
            _motherboardRepository = motherboardRepository;
            LoadParts();
        }

        protected override async void LoadParts()
        {
            _allParts.Clear();
            await foreach (var motherboard in _motherboardRepository.GetMotherboards())
            {
                _allParts.Add(new ComponentModel { Id = motherboard.Id, Name = motherboard.Name });
            }
            UpdateList();
        }
    }
}