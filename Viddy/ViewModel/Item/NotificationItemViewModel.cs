using VidMePortable.Model;

namespace Viddy.ViewModel.Item
{
    public class NotificationItemViewModel : ViewModelBase
    {
        public NotificationItemViewModel(Notification notification)
        {
            Notification = notification;
        }

        public Notification Notification { get; set; }

    }
}