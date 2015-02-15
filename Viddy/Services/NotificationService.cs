using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using PropertyChanged;
using Viddy.BackgroundTask;
using Viddy.Core.Services;
using Viddy.Views.Account;
using VidMePortable;

namespace Viddy.Services
{
    public interface INotificationService
    {
        void StartService();
        int NotificationCount { get; set; }
        bool DisplayNotificationCount { get; }
        Task<bool> MarkAllAsRead();
    }

    [ImplementPropertyChanged]
    public class NotificationService : INotificationService
    {
        private readonly IVidMeClient _vidMeClient;
        private readonly ITileService _tileService;
        private readonly INavigationService _navigationService;
        private readonly DispatcherTimer _timer;
        private readonly NotificationTask _notificationTask;

#if DEBUG
        private const double Interval = 0.5;
#else
        private const int Interval = 15;
#endif
        
        public NotificationService(INavigationService navigationService, IVidMeClient vidMeClient, ITileService tileService)
        {
            _vidMeClient = vidMeClient;
            _tileService = tileService;
            _navigationService = navigationService;
            _notificationTask = new NotificationTask();

            _timer = new DispatcherTimer {Interval = TimeSpan.FromMinutes(Interval)};
            _timer.Tick += TimerOnTick;
        }

        private void TimerOnTick(object sender, object o)
        {
            CheckForNotifications();
        }

        private async Task CheckForNotifications()
        {
            NotificationCount = await _notificationTask.CheckForNotifications(false);
        }

        public void StartService()
        {
            if (_timer != null && !_timer.IsEnabled)
            {
                _timer.Start();
            }

            AuthenticationService.Current.UserSignedIn += CurrentOnUserSignedIn;
            AuthenticationService.Current.UserSignedOut += CurrentOnUserSignedIn;

            CheckForNotifications();
        }

        private void CurrentOnUserSignedIn(object sender, EventArgs eventArgs)
        {
            if (AuthenticationService.Current.IsLoggedIn)
            {
                if (_timer != null && !_timer.IsEnabled)
                {
                    _timer.Start();
                    CheckForNotifications();
                }
            }
            else
            {
                if (_timer != null && _timer.IsEnabled)
                {
                    _timer.Stop();
                }

                _notificationTask.UpdateTileCount(0, false);
            }
        }

        public int NotificationCount { get; set; }

        public bool DisplayNotificationCount
        {
            get { return NotificationCount > 0; }
        }

        public async Task<bool> MarkAllAsRead()
        {
            try
            {
                if (await _vidMeClient.MarkAllNotificationsAsReadAsync())
                {
                    NotificationCount = 0;
                    _notificationTask.UpdateTileCount(0, false);
                    return true;
                }
            }
            catch (Exception ex)
            {
            }

            return false;
        }

        public RelayCommand NavigateToNotificationsCommand
        {
            get { return new RelayCommand(() => _navigationService.Navigate<NotificationsView>()); }
        }
    }
}
