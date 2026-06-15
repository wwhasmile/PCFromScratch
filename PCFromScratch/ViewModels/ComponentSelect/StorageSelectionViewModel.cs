using PCFromScratch.App.Utils;
using PCFromScratch.DTOModels;

namespace PCFromScratch.App.ViewModels
{
    public class StorageSelectionViewModel : BaseComponentViewModel<InternalDriveDtoModel>
    {
        private readonly ServerRequests _serverRequests;

        public StorageSelectionViewModel(ServerRequests serverRequests)
        {
            _serverRequests = serverRequests;
            _ = FetchParts();
        }

        protected override async Task FetchParts()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                _allParts = await _serverRequests.GetItems<InternalDriveDtoModel>("/drive") ?? new List<InternalDriveDtoModel>();
                LoadParts();
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}