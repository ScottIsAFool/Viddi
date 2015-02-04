using Cimbalino.Toolkit.Services;
using VidMePortable;

namespace Viddy.ViewModel
{
    public class BrowseChannelsViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IVidMeClient _vidMeClient;

        public BrowseChannelsViewModel(INavigationService navigationService, IVidMeClient vidMeClient)
        {
            _navigationService = navigationService;
            _vidMeClient = vidMeClient;
        }
    }
}
