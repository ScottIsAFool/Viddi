using System;
using System.IO;
using Windows.Storage;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Viddy.Common;
using Viddy.Extensions;
using Viddy.Services;
using Viddy.Views;
using VidMePortable;
using VidMePortable.Model.Requests;

namespace Viddy.ViewModel
{
    public class UploadVideoViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IVidMeClient _vidMeClient;
        private readonly IApplicationSettingsService _applicationSettings;
        private readonly FoursqureViewModel _foursqureViewModel;

        public UploadVideoViewModel(
            INavigationService navigationService, 
            IVidMeClient vidMeClient, 
            IApplicationSettingsService applicationSettings, 
            FoursqureViewModel foursqureViewModel,
            EditVideoViewModel editVideoViewModel)
        {
            EditVideo = editVideoViewModel;
            _navigationService = navigationService;
            _vidMeClient = vidMeClient;
            _applicationSettings = applicationSettings;
            _foursqureViewModel = foursqureViewModel;

            if (IsInDesignMode)
            {
                IsUploading = true;
            }
        }

        public EditVideoViewModel EditVideo { get; set; }
        public StorageFile File { get; set; }

        public bool Pause { get; set; }
        public bool Play { get; set; }

        public bool IsPlaying { get; set; }

        public bool IsUploading { get; set; }

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
                        IsUploading = true;
                        Pause = !Pause;
                        var request = await _vidMeClient.RequestVideoAsync(new VideoRequest
                        {
                            FourSquarePlaceId = _foursqureViewModel.VenueId,
                            FourSquarePlaceName = _foursqureViewModel.VenueName,
                            Latitude = _foursqureViewModel.Latitude,
                            Longitude = _foursqureViewModel.Longitude
                        });

                        if (request != null)
                        {
                            EditVideo.SetVideo(request.Video);
                            var stream = await File.OpenReadAsync();
                            var video = await _vidMeClient.UploadVideoAsync(request.Code, File.ContentType, File.Name, stream.AsStream());
                            if (video != null)
                            {
                                if (!AuthenticationService.Current.IsLoggedIn)
                                {
                                    // We need to save the token that comes back so we can use it to delete anonymous videos
                                    var key = Utils.GetAnonVideoKeyName(video.Id);
                                    _applicationSettings.Roaming.SetS(key, request.AccessToken);
                                }
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

                    IsUploading = true;
                    //EditVideo.CanEdit = IsUploading = false;
                    File = file;
                }
            });

            base.WireMessages();
        }
    }
}
