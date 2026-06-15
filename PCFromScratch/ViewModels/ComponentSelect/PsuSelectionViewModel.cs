using PCFromScratch.App.Utils;
using PCFromScratch.DTOModels;

namespace PCFromScratch.App.ViewModels
{
    public class PsuSelectionViewModel : BaseComponentViewModel<PsuDtoModel>
    {
        private readonly ServerRequests _serverRequests;

        public PsuSelectionViewModel(ServerRequests serverRequests)
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
                _allParts = await _serverRequests.GetItems<PsuDtoModel>("/psu") ?? new List<PsuDtoModel>();
                LoadParts();
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}