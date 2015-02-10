using Cimbalino.Toolkit.Services;
using Viddy.ViewModel.Item;
using VidMePortable;

namespace Viddy.ViewModel.Account
{
    public class NotificationsViewModel : LoadingItemsViewModel<NotificationItemViewModel>
    {
        private readonly INavigationService _navigationService;
        private readonly IVidMeClient _vidMeClient;

        public NotificationsViewModel(INavigationService navigationService, IVidMeClient vidMeClient)
        {
            _navigationService = navigationService;
            _vidMeClient = vidMeClient;
        }
    }
}
