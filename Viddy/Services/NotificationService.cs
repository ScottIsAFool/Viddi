using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using PropertyChanged;
using Viddy.Extensions;
using Viddy.Views.Account;
using VidMePortable;
using VidMePortable.Model;

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

        public NotificationService(INavigationService navigationService, IVidMeClient vidMeClient, ITileService tileService)
        {
            _vidMeClient = vidMeClient;
            _tileService = tileService;
            _navigationService = navigationService;
            NotificationCount = 0;
            _timer = new DispatcherTimer {Interval = TimeSpan.FromMinutes(15)};
            _timer.Tick += TimerOnTick;
        }

        private void TimerOnTick(object sender, object o)
        {
            CheckForNotifications();
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
            }
        }

        public int NotificationCount { get; set; }

        public bool DisplayNotificationCount
        {
            get { return NotificationCount > 0; }
        }

        public Task<bool> MarkAllAsRead()
        {
            return _vidMeClient.MarkAllNotificationsAsReadAsync();
        }

        public RelayCommand NavigateToNotificationsCommand
        {
            get { return new RelayCommand(() => _navigationService.Navigate<NotificationsView>()); }
        }

        private List<Notification> _notifications;
        private async Task CheckForNotifications(int limit = 1, int offset = 0)
        {
            if (!AuthenticationService.Current.IsLoggedIn)
            {
                UpdateTileCount(0);
                return;
            }

            try
            {
                var response = await _vidMeClient.GetNotificationsAsync(limit, offset);
                if (response != null && !response.Notifications.IsNullOrEmpty())
                {
                    if (limit == 1)
                    {
                        _notifications = null;
                        var notification = response.Notifications[0];
                        if (!notification.Read)
                        {
                            await CheckForNotifications(20);
                        }
                        else
                        {
                            UpdateTileCount(0);
                        }
                    }
                    else
                    {
                        if (_notifications == null)
                        {
                            _notifications = new List<Notification>();
                        }

                        _notifications.AddRange(response.Notifications);
                        var hasReadItems = _notifications.Any(x => x.Read);
                        if (hasReadItems)
                        {
                            UpdateTileCount(_notifications.Count(x => !x.Read));
                        }
                        else
                        {
                            await CheckForNotifications(20, _notifications.Count);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        private void UpdateTileCount(int count)
        {
            NotificationCount = count;
            _tileService.UpdateTileCount(count);
        }
    }
}
