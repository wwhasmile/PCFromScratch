using PCFromScratch.DBModels;
using PCFromScratch.Repository;

namespace PCFromScratch.App.ViewModels
{
    public class StorageSelectionViewModel : BaseComponentViewModel<InternalDrive>
    {
        private readonly IInternalDriveRepository _storageRepository;

        public StorageSelectionViewModel(IInternalDriveRepository storageRepository)
        {
            _storageRepository = storageRepository;
            LoadParts();
        }

        protected override async void LoadParts()
        {
            _allParts.Clear();
            await foreach (var storage in _storageRepository.GetInternalDrives())
            {
                _allParts.Add(storage);
            }
            UpdateList();
        }
    }
}