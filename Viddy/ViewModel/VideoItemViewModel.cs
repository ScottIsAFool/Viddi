using System;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Ioc;
using Viddy.Services;
using VidMePortable.Model;

namespace Viddy.ViewModel
{
    public class VideoItemViewModel : ViewModelBase
    {
        private readonly IApplicationSettingsService _settingsService;

        public VideoItemViewModel(Video video)
        {
            Video = video;
            _settingsService = SimpleIoc.Default.GetInstance<IApplicationSettingsService>();
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

        private bool VideoIsAnonymousButOwned()
        {
            // This means it's an anonymous video
            return string.IsNullOrEmpty(Video.UserId) && _settingsService.Roaming.Contains(Utils.GetAnonVideoKeyName(Video.VideoId));
        }
    }
}
