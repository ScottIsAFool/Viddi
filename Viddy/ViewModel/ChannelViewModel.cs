using System.Collections.Generic;
using System.Threading.Tasks;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Viddy.Messaging;
using Viddy.Services;
using Viddy.ViewModel.Item;
using VidMePortable;
using VidMePortable.Model;
using VidMePortable.Model.Responses;

namespace Viddy.ViewModel
{
    public class ChannelViewModel : ViewModelBase, IBackSupportedViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IVidMeClient _vidMeClient;
        private readonly ITileService _tileService;

        private Stack<ChannelItemViewModel> _previousItems; 

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

        public RelayCommand PageLoadedCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    if (Channel != null)
                    {
                        Channel.RefreshFollowerDetails().ConfigureAwait(false);
                        await Channel.PageLoaded();
                    }
                });
            }
        }

        protected override void WireMessages()
        {
            Messenger.Default.Register<ChannelMessage>(this, m =>
            {
                Channel = m.Channel;
            });

            base.WireMessages();
        }

        public void ChangeContext()
        {
            if (_previousItems == null || _previousItems.Count == 0)
            {
                return;
            }

            var item = _previousItems.Pop();
            if (item != null)
            {
                Channel = item;
            }
        }

        public void SaveContext()
        {
            if (_previousItems == null)
            {
                _previousItems = new Stack<ChannelItemViewModel>();
            }

            _previousItems.Push(Channel);
        }
    }
}
