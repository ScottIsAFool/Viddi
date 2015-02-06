using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Viddy.Messaging;
using Viddy.ViewModel.Item;
using Viddy.Views;
using Viddy.Views.Account;

namespace Viddy.ViewModel
{
    public class VideoPlayerViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        public VideoPlayerViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public VideoItemViewModel Video { get; set; }

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

                    Messenger.Default.Send(new UserMessage(Video.Video.User));
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
