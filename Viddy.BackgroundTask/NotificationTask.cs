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
using Viddy.Core;
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
                new AuthenticationService(_settingsService, _vidMeClient);
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

        private List<string> _previousToastIds;
        public void UpdateTileCount(int count, bool showToast)
        {
            if (showToast)
            {
                LoadPreviousToasts();
                ShowToasts();
                SavePreviousToasts();
            }

            if (count == 0)
            {
                ClearPreviousToasts();
                ToastNotificationManager.History.Clear();
            }

            // Update tile
            var badgeContent = new BadgeNumericNotificationContent((uint)count);
            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(badgeContent.CreateNotification());
        }

        private void ShowToasts()
        {
            var toastManager = ToastNotificationManager.CreateToastNotifier();
            // Show toast
            foreach (var notification in _notifications.Where(x => !x.Read))
            {
                if (_previousToastIds.Contains(notification.NotificationId))
                {
                    continue;
                }

                var toastNotification = ToastContentFactory.CreateToastText02();
                toastNotification.Launch = HandleNotificationType(notification);
                toastNotification.TextHeading.Text = "Viddy";
                toastNotification.TextBodyWrap.Text = notification.Text;
                
                var toast = toastNotification.CreateNotification();
                toast.Tag = notification.NotificationId;
                toastManager.Show(toast);

                _previousToastIds.Add(notification.NotificationId);
            }
        }

        private static string HandleNotificationType(Notification notification)
        {
            if (notification == null)
            {
                return "viddy://";
            }

            var type = notification.NotificationType;
            var urlTemplate = "viddy://{0}?id={1}&notificationId=" + notification.NotificationId;

            switch (type)
            {
                case NotificationType.ChannelSubscribed:
                    var channel = notification.Channel;
                    if (channel != null)
                    {
                        return string.Format(urlTemplate, "channel", channel.ChannelId);
                    }
                    break;
                case NotificationType.CommentReply:
                    break;
                case NotificationType.UserSubscribed:
                    var user = notification.User;
                    if (user != null)
                    {
                        return string.Format(urlTemplate, "user", user.UserId);
                    }
                    break;
                case NotificationType.VideoComment:
                case NotificationType.VideoUpVoted:
                    var video = notification.Video;
                    if (video != null)
                    {
                        return string.Format(urlTemplate, "video", video.VideoId);
                    }
                    break;
            }

            return "viddy://";
        }

        private void LoadPreviousToasts()
        {
            var list = _settingsService.Local.GetS<List<string>>(Constants.StorageSettings.PreviousToasts);
            _previousToastIds = list ?? new List<string>();
        }

        private void SavePreviousToasts()
        {
            if (!_previousToastIds.IsNullOrEmpty())
            {
                _settingsService.Local.SetS(Constants.StorageSettings.PreviousToasts, _previousToastIds);
            }
        }

        private void ClearPreviousToasts()
        {
            _settingsService.Local.Remove(Constants.StorageSettings.PreviousToasts);
        }
    }
}
