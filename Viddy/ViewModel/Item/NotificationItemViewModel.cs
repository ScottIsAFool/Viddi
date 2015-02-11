using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
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

                    break;
                case NotificationType.VideoUpVoted:

                    break;
            }
        }
    }
}