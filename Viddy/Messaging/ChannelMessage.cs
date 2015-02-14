using GalaSoft.MvvmLight.Messaging;
using Viddy.ViewModel.Item;

namespace Viddy.Messaging
{
    public class ChannelMessage : NotificationMessage
    {
        public ChannelItemViewModel Channel { get; set; }

        public ChannelMessage(ChannelItemViewModel channel) : base(string.Empty)
        {
            Channel = channel;
        }

        public ChannelMessage(ChannelItemViewModel channel, string notification) : base(notification)
        {
            Channel = channel;
        }
    }
}