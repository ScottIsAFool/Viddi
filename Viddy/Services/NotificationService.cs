using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using PropertyChanged;
using Viddy.Views.Account;
using VidMePortable;

namespace Viddy.Services
{
    public interface INotificationService
    {
        void StartService();
        int NotificationCount { get; set; }
        bool DisplayNotificationCount { get; }
    }

    [ImplementPropertyChanged]
    public class NotificationService : INotificationService
    {
        private readonly IVidMeClient _vidMeClient;
        private readonly INavigationService _navigationService;

        public NotificationService(IVidMeClient vidMeClient, INavigationService navigationService)
        {
            _vidMeClient = vidMeClient;
            _navigationService = navigationService;
            NotificationCount = 6;
        }

        public void StartService()
        {
        }

        public int NotificationCount { get; set; }

        public bool DisplayNotificationCount
        {
            get { return NotificationCount > 0; }
        }

        public RelayCommand NavigateToNotificationsCommand
        {
            get { return new RelayCommand(() => _navigationService.Navigate<NotificationsView>()); }
        }
    }
}
