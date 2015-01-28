using System;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Viddy.Extensions;
using Viddy.Messaging;
using Viddy.Services;
using Viddy.Views;
using VidMePortable;
using VidMePortable.Model;

namespace Viddy.ViewModel
{
    public class VideoItemViewModel : ViewModelBase, IListType
    {
        private readonly IVidMeClient _vidMeClient;
        private readonly VideoLoadingViewModel _videoLoadingViewModel;
        private readonly IApplicationSettingsService _settingsService;
        private readonly INavigationService _navigationService;

        public VideoItemViewModel(Video video, VideoLoadingViewModel videoLoadingViewModel)
        {
            _vidMeClient = SimpleIoc.Default.GetInstance<IVidMeClient>();
            _settingsService = SimpleIoc.Default.GetInstance<IApplicationSettingsService>();
            _navigationService = SimpleIoc.Default.GetInstance<INavigationService>();
            _videoLoadingViewModel = videoLoadingViewModel;
            Video = video;
        }

        public Video Video { get; set; }

        public bool CanDelete
        {
            get
            {
                return Video != null
                       && (AuthenticationService.Current.IsLoggedIn
                       && AuthenticationService.Current.LoggedInUserId == Video.UserId)
                       || VideoIsAnonymousButOwned();
            }
        }

        public bool DisplayDuration
        {
            get { return Video != null && Video.Duration.HasValue; }
        }

        public string VideoLength
        {
            get
            {
                if (IsInDesignMode)
                {
                    return "0:07";
                }

                if (Video == null || !Video.Duration.HasValue)
                {
                    return string.Empty;
                }

                var ts = TimeSpan.FromSeconds(Video.Duration.Value);
                if (ts.TotalHours > 1)
                {
                    return string.Format("{0:c}", ts);
                }

                return string.Format("{0:0}:{1:00}", ts.Minutes, ts.Seconds);
            }
        }

        public RelayCommand DeleteCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    if (Video == null || !CanDelete)
                    {
                        return;
                    }

                    try
                    {
                        if (AuthenticationService.Current.IsLoggedIn)
                        {
                            if (await _vidMeClient.DeleteVideoAsync(Video.VideoId))
                            {
                                _videoLoadingViewModel.Videos.Remove(this);
                            }
                        }
                        else
                        {
                            var key = Utils.GetAnonVideoKeyName(Video.VideoId);
                            var auth = _settingsService.Roaming.GetS<Auth>(key);

                            if (auth != null)
                            {
                                if (await _vidMeClient.DeleteAnonymousVideoAsync(Video.VideoId, auth.Token))
                                {
                                    _videoLoadingViewModel.Videos.Remove(this);
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

        public RelayCommand OpenVideoCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Messenger.Default.Send(new VideoMessage(this));
                    _navigationService.Navigate<VideoPlayerView>();
                });
            }
        }

        private bool VideoIsAnonymousButOwned()
        {
            // This means it's an anonymous video
            return string.IsNullOrEmpty(Video.UserId) && _settingsService.Roaming.Contains(Utils.GetAnonVideoKeyName(Video.VideoId));
        }

        public ListType ListType { get { return ListType.Normal; } }
    }
}
