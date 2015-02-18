using System;
using System.IO;
using Windows.Storage;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Viddy.Common;
using Viddy.Core;
using Viddy.Core.Extensions;
using Viddy.Core.Services;
using Viddy.Messaging;
using Viddy.ViewModel.Item;
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
        public object Image { get; set; }
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
                        SetProgressBar("Uploading video...");
                        IsUploading = true;
                        EditVideo.SetIsUploading(true);
                        Pause = !Pause;
                        var request = await _vidMeClient.RequestVideoAsync(new VideoRequest
                        {
                            FourSquarePlaceId = _foursqureViewModel.VenueId,
                            FourSquarePlaceName = _foursqureViewModel.VenueName,
                            Latitude = _foursqureViewModel.Latitude,
                            Longitude = _foursqureViewModel.Longitude,
                            ChannelId = _channel != null ? _channel.Id : null
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
                        Log.ErrorException("UploadVideoCommand()", ex);
                        EditVideo.ErrorMessage = "There was an error trying to upload your video";
                    }

                    SetProgressBar();
                    EditVideo.SetIsUploading(false);
                    IsUploading = false;
                });
            }
        }

        private ChannelItemViewModel _channel;

        protected override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, m =>
            {
                if (m.Notification.Equals(Constants.Messages.VideoFileMsg))
                {
                    var file = m.Sender as StorageFile;
                    if (file == null) return;

                    _channel = m.Target != null ? m.Target as ChannelItemViewModel : null;

                    //IsUploading = true;
                    EditVideo.CanEdit = IsUploading = false;
                    File = file;
                    Image = File;
                }
            });

            Messenger.Default.Register<VideoMessage>(this, m =>
            {
                if (m.Notification.Equals(Constants.Messages.EditVideoMsg))
                {
                    EditVideo.SetVideo(m.Video.Video);
                    Image = m.Video.Video.ThumbnailUrl;

                }
            });

            base.WireMessages();
        }
    }
}
