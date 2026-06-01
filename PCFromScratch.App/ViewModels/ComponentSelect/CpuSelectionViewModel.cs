using PCFromScratch.Repository;

namespace PCFromScratch.App.ViewModels
{
    public class CpuSelectionViewModel : BaseComponentViewModel
    {
        private readonly ICpuRepository _cpuRepository;

        public CpuSelectionViewModel(ICpuRepository cpuRepository)
        {
            _cpuRepository = cpuRepository;
            LoadParts();
        }

        protected override async void LoadParts()
        {
            _allParts.Clear();
            await foreach (var cpu in _cpuRepository.GetCpus())
            {
                _allParts.Add(new ComponentModel { Id = cpu.Id, Name = cpu.Name });
            }
            UpdateList();
        }
    }
}