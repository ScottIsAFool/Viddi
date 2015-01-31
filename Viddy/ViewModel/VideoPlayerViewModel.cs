using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Viddy.Messaging;
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
                        await Video.LoadData(false);
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

        protected override void WireMessages()
        {
            Messenger.Default.Register<VideoMessage>(this, m =>
            {
                Video = m.Video;
            });
        }
    }
}
