using System;
using System.IO;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Viddy.Common;
using Viddy.Views;
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
        public Stream VideoStream { get; set; }
        public string VideoToPlay { get; set; }
        public StorageFile File { get; set; }

        public RelayCommand CancelCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    // TODO: Clear the property
                    _selectedVideoFile = null;
                    if (_navigationService.CanGoBack)
                    {
                        _navigationService.GoBack();
                    }
                    else
                    {
                        _navigationService.Navigate<MainPage>(new NavigationParameters {ClearBackstack = true});
                    }

                });
            }
        }

        protected override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, async m =>
            {
                if (m.Notification.Equals(Constants.Messages.VideoFileMsg))
                {
                    var file = m.Sender as StorageFile;
                    if (file == null) return;

                    File = file;
                    VideoToPlay = "ms-appx:///" + file.FolderRelativeId;
                    //var thumbnail = await file.GetThumbnailAsync(ThumbnailMode.VideosView);
                    //Thumbnail = thumbnail;
                }
            });
        }
    }
}
