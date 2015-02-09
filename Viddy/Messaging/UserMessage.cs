using GalaSoft.MvvmLight.Messaging;
using Viddy.ViewModel.Item;

namespace Viddy.Messaging
{
    public class UserMessage : NotificationMessage
    {
        public UserViewModel User { get; set; }

        public UserMessage(UserViewModel user) : base(string.Empty)
        {
            User = user;
        }

        public UserMessage(UserViewModel user, string notification) : base(notification)
        {
            User = user;
        }
    }
}
