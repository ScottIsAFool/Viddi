using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Viddy.Core;
using Viddy.Messaging;
using Viddy.Views;
using Viddy.Views.Account;
using VidMePortable.Model;

namespace Viddy.ViewModel.Item
{
    public class NotificationItemViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        public NotificationItemViewModel(Notification notification)
        {
            Notification = notification;
            _navigationService = SimpleIoc.Default.GetInstance<INavigationService>();
        }

        public Notification Notification { get; set; }

        public string NotificationText
        {
            get { return Notification != null ? Notification.Text : string.Empty; }
        }

        public string AvatarUrl
        {
            get
            {
                if (Notification == null)
                {
                    return string.Empty;
                }

                var type = Notification.NotificationType;

                switch (type)
                {
                    case NotificationType.ChannelSubscribed:
                        var channel = Notification.Channel;
                        if (channel != null)
                        {
                            return channel.AvatarUrl;
                        }
                        break;
                    case NotificationType.CommentReply:
                    case NotificationType.UserSubscribed:
                    case NotificationType.VideoComment:
                    case NotificationType.VideoUpVoted:
                        var user = Notification.User;
                        if (user != null)
                        {
                            return user.AvatarUrl;
                        }
                        break;
                    default:
                        return string.Empty;
                }

                return string.Empty;
            }
        }

        public string NotificationDate
        {
            get { return Notification != null && Notification.DateCreated.HasValue ? Utils.DaysAgo(Notification.DateCreated.Value) : string.Empty; }
        }

        public RelayCommand NavigationActionCommand
        {
            get
            {
                return new RelayCommand(HandleNotificationType);
            }
        }

        private void HandleNotificationType()
        {
            if (Notification == null)
            {
                return;
            }

            var type = Notification.NotificationType;

            switch (type)
            {
                case NotificationType.ChannelSubscribed:
                    var channel = Notification.Channel;
                    if (channel != null)
                    {
                        Messenger.Default.Send(new ChannelMessage(new ChannelItemViewModel(channel)));
                        _navigationService.Navigate<ChannelView>();
                    }
                    break;
                case NotificationType.CommentReply:

                    break;
                case NotificationType.UserSubscribed:
                    var user = Notification.User;
                    if (user != null)
                    {
                        Messenger.Default.Send(new UserViewModel(user));
                        _navigationService.Navigate<ProfileView>();
                    }
                    break;
                case NotificationType.VideoComment:
                case NotificationType.VideoUpVoted:
                    var video = Notification.Video;
                    if (video != null)
                    {
                        Messenger.Default.Send(new VideoMessage(new VideoItemViewModel(video, null)));
                        _navigationService.Navigate<VideoPlayerView>();
                    }
                    break;
            }
        }
    }
}