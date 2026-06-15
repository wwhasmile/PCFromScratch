using PCFromScratch.App.Utils;
using PCFromScratch.DTOModels;

namespace PCFromScratch.App.ViewModels
{
    public class CoolerSelectionViewModel : BaseComponentViewModel<CoolerDtoModel>
    {
        private readonly ServerRequests _serverRequests;

        public CoolerSelectionViewModel(ServerRequests serverRequests)
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
                _allParts = await _serverRequests.GetItems<CoolerDtoModel>("/cooler") ?? new List<CoolerDtoModel>();
                LoadParts();
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}