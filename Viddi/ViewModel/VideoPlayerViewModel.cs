﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ScottIsAFool.Windows.Core.Extensions;
using ScottIsAFool.Windows.Core.ViewModel;
using Viddi.Core.Extensions;
using Viddi.Core.Model;
using Viddi.Messaging;
using Viddi.Services;
using Viddi.ViewModel.Item;
using Viddi.Views;
using Viddi.Views.Account;
using VidMePortable;
using VidMePortable.Model;

namespace Viddi.ViewModel
{
    public class VideoPlayerViewModel : ViewModelBase, IBackSupportedViewModel, ICanHasHomeButton
    {
        private readonly INavigationService _navigationService;
        private readonly IVidMeClient _vidMeClient;
        private readonly ITileService _tileService;

        private Stack<VideoItemViewModel> _previousItems;
        private bool _fromProtocol;

        public VideoPlayerViewModel(INavigationService navigationService, IVidMeClient vidMeClient, ITileService tileService)
        {
            _navigationService = navigationService;
            _vidMeClient = vidMeClient;
            _tileService = tileService;
        }

        public VideoItemViewModel Video { get; set; }

        public override bool IsPinned
        {
            get { return Video != null && Video.IsPinned; }
        }

        public override string GetPinFileName(bool isWideTile = false)
        {
            return _tileService.GetTileFileName(TileType.Video, Video.Video.VideoId, isWideTile);
        }

        public override async Task PinUnpin()
        {
            if (IsPinned)
            {
                await _tileService.UnpinVideo(Video.Video.VideoId);
            }
            else
            {
                await _tileService.PinVideo(Video.Video);
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
                        await LoadVideoData();
                    }
                });
            }
        }

        private async Task LoadVideoData()
        {
            if (Video != null)
            {
                Video.RefreshVideoDetails().ConfigureAwait(false);
                await Video.LoadData();
            }
        }

        public RelayCommand NavigateToUserCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (Video == null || Video.IsAnonymous)
                    {
                        return;
                    }

                    Messenger.Default.Send(new UserMessage(new UserViewModel(Video.Video.User)));
                    _navigationService.Navigate<ProfileView>();
                });
            }
        }

        public RelayCommand NavigateToChannelCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Messenger.Default.Send(new ChannelMessage(Video.Channel));
                    _navigationService.Navigate<ChannelView>();
                });
            }
        }

        protected override void WireMessages()
        {
            Messenger.Default.Register<VideoMessage>(this, m =>
            {
                _fromProtocol = false;
                Video = m.Video;
            });

            Messenger.Default.Register<ProtocolMessage>(this, m =>
            {
                if (m.Type != ProtocolMessage.ProtocolType.Video)
                {
                    return;
                }

                _fromProtocol = true;
                Video = new VideoItemViewModel(new Video(), null) {ProgressIsVisible = true};

                GetVideo(m.Content);
            });

            base.WireMessages();
        }

        private async Task GetVideo(string videoId)
        {
            try
            {
                var response = await _vidMeClient.GetVideoAsync(videoId);
                if (response != null)
                {
                    if (Video == null)
                    {
                        Video = new VideoItemViewModel(response.Video, null);
                    }
                    else
                    {
                        Video.Video = response.Video;
                    }

                    await LoadVideoData();
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        public void ChangeContext(Type callingType)
        {
            if (_previousItems.IsNullOrEmpty() || callingType != typeof(VideoPlayerView))
            {
                return;
            }

            var item = _previousItems.Pop();
            if (item != null)
            {
                Video = item;
            }
        }

        public void SaveContext()
        {
            if (_previousItems == null)
            {
                _previousItems = new Stack<VideoItemViewModel>();
            }

            _previousItems.Push(Video);
        }

        public bool ShowHomeButton { get; set; }

        public ICommand NavigateHomeCommand
        {
            get
            {
                return new RelayCommand(() => _navigationService.Navigate<MainView>());
            }
        }
    }
}
