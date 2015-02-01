using VidMePortable.Model;

namespace Viddy.ViewModel.Item
{
    public class ChannelItemViewModel : ViewModelBase
    {
        public ChannelItemViewModel(Channel channel)
        {
            Channel = channel;
        }

        public Channel Channel { get; set; }

        public string Name
        {
            get { return Channel != null ? Channel.Title : null; }
        }
    }
}
