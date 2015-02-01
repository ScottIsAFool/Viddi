using GalaSoft.MvvmLight.Messaging;
using Viddy.ViewModel.Item;

namespace Viddy.Messaging
{
    public class ChannelMessage : MessageBase
    {
        public ChannelItemViewModel Channel { get; set; }

        public ChannelMessage(ChannelItemViewModel channel)
        {
            Channel = channel;
        }
    }
}