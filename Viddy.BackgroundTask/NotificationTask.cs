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
                _vidMeClient = new VidMeClient(Utils.UniqueDeviceIdentifier, "WindowsPhone");
                new AuthenticationService(_settingsService, _vidMeClient);
            }

            if (AuthenticationService.Current != null)
            {
                if (!AuthenticationService.Current.ServiceStarted)
                {
                    AuthenticationService.Current.StartService();
                }

                if (_vidMeClient == null || _vidMeClient.AuthenticationInfo == null)
                {
                    _vidMeClient = AuthenticationService.Current.GetAuthenticatedVidMeClient();
                }
            }
        }

        public IAsyncOperation<int> CheckForNotifications(bool showToasts)
        {
            SetAuthentication();
            var task = CheckForNotificationsInternal(showToasts: showToasts);
            var returnTask = task.AsAsyncOperation();
            return returnTask;
        }

        private async Task<int> CheckForNotificationsInternal(bool showToasts = false)
        {
            var unreadCount = 0;
            if (!AuthenticationService.Current.IsLoggedIn)
            {
                UpdateTileCount(0, false);
                return unreadCount;
            }

            try
            {
                var response = await _vidMeClient.GetUnreadNotificationCountAsync();
                unreadCount = response;

                if (showToasts && unreadCount > 0)
                {
                    var notifications = await _vidMeClient.GetNotificationsAsync(unreadCount);
                    if (notifications != null && !notifications.Notifications.IsNullOrEmpty())
                    {
                        if (_notifications == null)
                        {
                            _notifications = notifications.Notifications;
                        }
                    }
                }

                UpdateTileCount(unreadCount, unreadCount > 0 && showToasts);
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
                toastNotification.Launch = HandleNotificationType(notification, toastNotification);
                toastNotification.TextBodyWrap.Text = notification.Text;

                var toast = toastNotification.CreateNotification();
                toast.Tag = notification.NotificationId;
                toastManager.Show(toast);

                _previousToastIds.Add(notification.NotificationId);
            }
        }

        private static string HandleNotificationType(Notification notification, IToastText02 toastNotification)
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
                    toastNotification.TextHeading.Text = "Channel subscription";

                    var channel = notification.Channel;
                    if (channel != null)
                    {
                        return string.Format(urlTemplate, "channel", channel.ChannelId);
                    }
                    break;
                case NotificationType.UserSubscribed:
                    toastNotification.TextHeading.Text = "User subscription";
                    var user = notification.User;
                    if (user != null)
                    {
                        return string.Format(urlTemplate, "user", user.UserId);
                    }
                    break;
                case NotificationType.CommentReply:
                case NotificationType.VideoComment:
                case NotificationType.VideoUpVoted:
                    if (type == NotificationType.CommentReply) toastNotification.TextHeading.Text = "Comment Reply";
                    else if (type == NotificationType.VideoComment) toastNotification.TextHeading.Text = "Video comment";
                    else if (type == NotificationType.VideoUpVoted) toastNotification.TextHeading.Text = "Video Up Vote";

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
