using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.UI.Notifications;
using Cimbalino.Toolkit.Services;
using NotificationsExtensions.BadgeContent;
using NotificationsExtensions.ToastContent;
using Viddy.Core.Extensions;
using Viddy.Core.Services;
using VidMePortable;
using VidMePortable.Model;

namespace Viddy.BackgroundTask
{
    public sealed class NotificationTask : IBackgroundTask
    {
        private IVidMeClient _vidMeClient;
        private readonly IApplicationSettingsService _settingsService = new ApplicationSettingsService();

        private AuthenticationService _authenticationService;

        private List<Notification> _notifications;
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();

            await CheckForNotifications(true);

            deferral.Complete();
        }

        private void SetAuthentication()
        {
            if (AuthenticationService.Current == null)
            {
                _vidMeClient = new VidMeClient(Utils.DeviceId, "WindowsPhone");
                _authenticationService = new AuthenticationService(_settingsService, _vidMeClient);
            }

            if (AuthenticationService.Current != null)
            {
                AuthenticationService.Current.StartService();
                _vidMeClient = AuthenticationService.Current.GetAuthenticatedVidMeClient();
            }
        }

        public IAsyncOperation<int> CheckForNotifications(bool showToasts)
        {
            SetAuthentication();
            var task = CheckForNotificationsInternal(showToasts: showToasts);
            var returnTask = task.AsAsyncOperation();
            return returnTask;
        }

        public IAsyncOperation<int> CheckForNotifications(int limit, int offset, bool showToasts)
        {
            SetAuthentication();
            var task = CheckForNotificationsInternal(limit, offset);
            var returnTask = task.AsAsyncOperation();
            return returnTask;
        }

        private async Task<int> CheckForNotificationsInternal(int limit = 1, int offset = 0, bool showToasts = false)
        {
            var unreadCount = 0;
            if (!AuthenticationService.Current.IsLoggedIn)
            {
                UpdateTileCount(0, false);
                return unreadCount;
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
                            unreadCount = await CheckForNotificationsInternal(20, showToasts: showToasts);
                        }
                        else
                        {
                            UpdateTileCount(0, false);
                            return unreadCount;
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
                            unreadCount = _notifications.Count(x => !x.Read);
                            UpdateTileCount(unreadCount, showToasts);
                            return unreadCount;
                        }
                        else
                        {
                            unreadCount = await CheckForNotifications(20, _notifications.Count, showToasts);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return unreadCount;
        }

        public void UpdateTileCount(int count, bool showToast)
        {
            if (showToast)
            {
                // Show toast
                var toastNotification = ToastContentFactory.CreateToastText02();
                toastNotification.TextHeading.Text = "Viddy";
                toastNotification.TextBodyWrap.Text = count > 1 ? string.Format("You have {0} unread notifications", count) : "You have 1 unread notification";
                ToastNotificationManager.CreateToastNotifier().Show(toastNotification.CreateNotification());
            }

            // Update tile
            var badgeContent = new BadgeNumericNotificationContent((uint)count);
            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(badgeContent.CreateNotification());
        }
    }
}
