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
                       && AuthenticationService.Current.IsLoggedIn
                       && AuthenticationService.Current.LoggedInUserId == Video.UserId
                       && VideoIsAnonymousButOwned();
            }
        }

        private bool VideoIsAnonymousButOwned()
        {
            // This means it's an anonymous video
            return string.IsNullOrEmpty(Video.UserId) && _settingsService.Local.Contains(Video.VideoId);
        }
    }
}
