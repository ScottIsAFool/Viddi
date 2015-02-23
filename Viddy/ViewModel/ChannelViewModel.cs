using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Viddy.Core;
using Viddy.Core.Extensions;
using Viddy.Core.Model;
using Viddy.Extensions;
using Viddy.Messaging;
using Viddy.Services;
using Viddy.ViewModel.Item;
using Viddy.Views;
using VidMePortable;
using VidMePortable.Model;

namespace Viddy.ViewModel
{
    public class ChannelViewModel : ViewModelBase, IBackSupportedViewModel, ICanHasHomeButton
    {
        private readonly INavigationService _navigationService;
        private readonly IVidMeClient _vidMeClient;
        private readonly ITileService _tileService;

        private Stack<ChannelItemViewModel> _previousItems;
        private bool _fromProtocol;

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
            return _tileService.GetTileFileName(TileType.Channel, Channel.Channel.ChannelId, isWideTile);
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
                    if (!_fromProtocol)
                    {
                        await LoadChannelVideos();
                    }
                });
            }
        }

        public RelayCommand AddVideoCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (App.Locator.VideoRecord != null)
                    {
                        Messenger.Default.Send(new ChannelMessage(Channel, Constants.Messages.AddVideoToChannelMsg));
                        _navigationService.Navigate<VideoRecordView>();
                    }
                });
            }
        }

        private async Task LoadChannelVideos()
        {
            if (Channel != null)
            {
                Channel.RefreshFollowerDetails().ConfigureAwait(false);
                await Channel.PageLoaded();
            }
        }

        private async Task GetChannel(string channelId)
        {
            try
            {
                var response = await _vidMeClient.GetChannelAsync(channelId);
                if (response != null)
                {
                    if (Channel == null)
                    {
                        Channel = new ChannelItemViewModel(response.Channel);
                    }
                    else
                    {
                        Channel.Channel = response.Channel;
                    }

                    await LoadChannelVideos();
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        protected override void WireMessages()
        {
            Messenger.Default.Register<ChannelMessage>(this, m =>
            {
                _fromProtocol = false;
                Channel = m.Channel;
            });

            Messenger.Default.Register<ProtocolMessage>(this, m =>
            {
                if (m.Type != ProtocolMessage.ProtocolType.Channel)
                {
                    return;
                }

                _fromProtocol = true;
                Channel = new ChannelItemViewModel(new Channel()) {ProgressIsVisible = true};
                GetChannel(m.Content);
            });

            base.WireMessages();
        }

        public void ChangeContext(Type callingType)
        {
            if (_previousItems.IsNullOrEmpty() || callingType != typeof(ChannelView))
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

        #region ICanHasHomeButton implementation
        public bool ShowHomeButton { get; set; }

        public RelayCommand NavigateHomeCommand
        {
            get
            {
                return new RelayCommand(() => _navigationService.Navigate<MainView>());
            }
        }
        #endregion
    }
}
