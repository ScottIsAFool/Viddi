using System;
using System.IO;
using Windows.Storage;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Viddy.Common;
using Viddy.Views;
using VidMePortable;
using VidMePortable.Model.Requests;
using VidMePortable.Model.Responses;

namespace Viddy.ViewModel
{
    public class UploadVideoViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IVidMeClient _vidMeClient;

        public UploadVideoViewModel(INavigationService navigationService, IVidMeClient vidMeClient)
        {
            _navigationService = navigationService;
            _vidMeClient = vidMeClient;
        }

        public StorageFile File { get; set; }

        public bool Pause { get; set; }
        public bool Play { get; set; }

        public bool IsPlaying { get; set; }

        public RelayCommand PlayPauseCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (IsPlaying)
                    {
                        Pause = !Pause;
                    }
                    else
                    {
                        Play = !Play;
                    }
                });
            }
        }

        public RelayCommand CancelCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    File = null;
                    if (_navigationService.CanGoBack)
                    {
                        _navigationService.GoBack();
                    }
                    else
                    {
                        _navigationService.Navigate<VideoRecordView>(new NavigationParameters {ClearBackstack = true});
                    }
                });
            }
        }

        public RelayCommand UploadVideoCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    try
                    {
                        Pause = !Pause;
                        var request = await _vidMeClient.RequestVideoAsync(new VideoRequest());
                        if (request != null)
                        {
                            var stream = await File.OpenReadAsync();
                            var video = await _vidMeClient.UploadVideoAsync(request.Code, File.ContentType, File.Name, stream.AsStream());
                            if (video != null)
                            {

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        
                    }
                });
            }
        }

        protected override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, m =>
            {
                if (m.Notification.Equals(Constants.Messages.VideoFileMsg))
                {
                    var file = m.Sender as StorageFile;
                    if (file == null) return;

                    File = file;
                }
            });
        }
    }
}
