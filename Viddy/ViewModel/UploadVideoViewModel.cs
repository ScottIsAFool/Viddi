using System;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Messaging;
using VidMePortable;

namespace Viddy.ViewModel
{
    public class UploadVideoViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IVidMeClient _vidMeClient;

        private StorageFile _selectedVideoFile;

        public UploadVideoViewModel(INavigationService navigationService, IVidMeClient vidMeClient)
        {
            _navigationService = navigationService;
            _vidMeClient = vidMeClient;
        }

        public StorageItemThumbnail Thumbnail { get; set; }

        protected override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, async m =>
            {
                if (m.Notification.Equals(Constants.Messages.VideoFileMsg))
                {
                    var file = m.Sender as StorageFile;
                    if (file == null) return;

                    _selectedVideoFile = file;
                    var thumbnail = await file.GetThumbnailAsync(ThumbnailMode.VideosView);
                    Thumbnail = thumbnail;
                }
            });
        }
    }
}
