using GalaSoft.MvvmLight.Messaging;
using VidMePortable.Model;

namespace Viddy.Messaging
{
    public class UserMessage : MessageBase
    {
        public User User { get; set; }

        public UserMessage(User user)
        {
            User = user;
        }
    }
}
