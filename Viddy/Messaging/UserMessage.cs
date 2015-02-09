using GalaSoft.MvvmLight.Messaging;
using Viddy.ViewModel.Item;

namespace Viddy.Messaging
{
    public class UserMessage : MessageBase
    {
        public UserViewModel User { get; set; }

        public UserMessage(UserViewModel user)
        {
            User = user;
        }
    }
}
