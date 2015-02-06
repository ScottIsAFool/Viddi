using System.Threading.Tasks;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Messaging;
using Viddy.Messaging;
using Viddy.ViewModel.Item;
using VidMePortable;
using VidMePortable.Model;
using VidMePortable.Model.Responses;

namespace Viddy.ViewModel
{
    public class ChannelViewModel : VideoLoadingViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IVidMeClient _vidMeClient;

        public ChannelViewModel(INavigationService navigationService, IVidMeClient vidMeClient)
        {
            _navigationService = navigationService;
            _vidMeClient = vidMeClient;
        }

        public ChannelItemViewModel Channel { get; set; }
        public ChannelItemViewModel EmptyChannel { get { return new ChannelItemViewModel(new Channel()); } }

        public override Task PageLoaded()
        {
            if (Channel != null)
            {
                Channel.RefreshFollowerDetails().ConfigureAwait(false);
            }

            return base.PageLoaded();
        }

        public override Task<VideosResponse> GetVideos(int offset)
        {
            return _vidMeClient.GetChannelsNewVideosAsync(Channel.Channel.ChannelId, offset);
        }

        protected override void WireMessages()
        {
            Messenger.Default.Register<ChannelMessage>(this, m =>
            {
                Channel = m.Channel;
            });

            base.WireMessages();
        }
    }
}
