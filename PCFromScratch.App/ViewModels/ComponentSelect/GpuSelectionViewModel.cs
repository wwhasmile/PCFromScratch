using PCFromScratch.Repository;

namespace PCFromScratch.App.ViewModels
{
    public class GpuSelectionViewModel : BaseComponentViewModel
    {
        private readonly IGpuRepository _gpuRepository;

        public GpuSelectionViewModel(IGpuRepository gpuRepository)
        {
            _gpuRepository = gpuRepository;
            LoadParts();
        }

        protected override async void LoadParts()
        {
            _allParts.Clear();
            await foreach (var gpu in _gpuRepository.GetGpus())
            {
                _allParts.Add(new ComponentModel { Id = gpu.Id, Name = gpu.Name });
            }
            UpdateList();
        }
    }
}