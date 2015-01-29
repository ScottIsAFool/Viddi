using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Viddy.Messaging;

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
                        await Video.LoadData(false);
                    }
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
