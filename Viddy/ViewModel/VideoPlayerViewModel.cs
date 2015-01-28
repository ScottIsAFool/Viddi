using System.Collections.Generic;
using System.Collections.ObjectModel;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Messaging;
using Viddy.Messaging;
using VidMePortable;

namespace Viddy.ViewModel
{
    public class VideoPlayerViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IVidMeClient _vidMeClient;

        public VideoPlayerViewModel(INavigationService navigationService, IVidMeClient vidMeClient)
        {
            _navigationService = navigationService;
            _vidMeClient = vidMeClient;
        }

        public VideoItemViewModel Video { get; set; }
        public ObservableCollection<IListType> Comments { get; set; }
        public bool IsEmpty { get; set; }
        public bool CanLoadMore { get; set; }
        public bool IsLoadingMore { get; set; }

        protected override void WireMessages()
        {
            Messenger.Default.Register<VideoMessage>(this, m =>
            {
                Video = m.Video;
                Reset();
            });
        }

        private void Reset()
        {
            Comments = null;
            IsEmpty = true;
            CanLoadMore = false;
            IsLoadingMore = false;
        }
    }
}
