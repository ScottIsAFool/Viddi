﻿using System.Threading.Tasks;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Viddy.Messaging;
using Viddy.Services;
using Viddy.ViewModel.Item;
using Viddy.Views;
using Viddy.Views.Account;

namespace Viddy.ViewModel
{
    public class VideoPlayerViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly ITileService _tileService;

        public VideoPlayerViewModel(INavigationService navigationService, ITileService tileService)
        {
            _navigationService = navigationService;
            _tileService = tileService;
        }

        public VideoItemViewModel Video { get; set; }

        public override bool IsPinned
        {
            get { return Video != null && Video.IsPinned; }
        }

        public override string GetPinFileName(bool isWideTile = false)
        {
            return _tileService.GetTileFileName(TileService.TileType.Video, Video.Video.VideoId, isWideTile);
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
                    if (Video != null)
                    {
                        Video.RefreshVideoDetails().ConfigureAwait(false);
                        await Video.LoadData();
                    }
                });
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
                Video = m.Video;
            });

            base.WireMessages();
        }
    }
}
