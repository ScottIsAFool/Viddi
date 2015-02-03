using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using ThemeManagerRt;
using VidMePortable;

namespace Viddy.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IVidMeClient _vidMeClient;
        private readonly IApplicationSettingsService _settingsService;

        public SettingsViewModel(INavigationService navigationService, IVidMeClient vidMeClient, IApplicationSettingsService settingsService)
        {
            _navigationService = navigationService;
            _vidMeClient = vidMeClient;
            _settingsService = settingsService;
        }

        public bool IsLightTheme { get; set; }

        public RelayCommand ToLightCommand
        {
            get
            {
                return new RelayCommand(ThemeManager.ToLightTheme);
            }
        }

        public RelayCommand ToDarkCommand
        {
            get
            {
                return new RelayCommand(ThemeManager.ToDarkTheme);
            }
        }
    }
}
