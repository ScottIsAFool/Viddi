using System.Threading.Tasks;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Messaging;
using Viddy.Messaging;
using Viddy.Services;
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
        private readonly ITileService _tileService;

        public ChannelViewModel(INavigationService navigationService, IVidMeClient vidMeClient, ITileService tileService)
        {
            _navigationService = navigationService;
            _vidMeClient = vidMeClient;
            _tileService = tileService;
        }

        public ChannelItemViewModel Channel { get; set; }
        public ChannelItemViewModel EmptyChannel { get { return new ChannelItemViewModel(new Channel()); } }

        public override bool IsPinned
        {
            get { return Channel != null && Channel.IsPinned; }
        }

        public override string GetPinFileName(bool isWideTile = false)
        {
            return _tileService.GetTileFileName(TileService.TileType.Channel, Channel.Channel.ChannelId, isWideTile);
        }

        public override async Task PinUnpin()
        {
            if (IsPinned)
            {
                await _tileService.UnpinChannel(Channel.Channel.ChannelId);
            }
            else
            {
                await _tileService.PinChannel(Channel.Channel);
            }

            RaisePropertyChanged(() => IsPinned);
        }

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
