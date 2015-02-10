using Windows.UI.Xaml;
using GalaSoft.MvvmLight.Command;
using JetBrains.Annotations;
using ThemeManagerRt;
using Viddy.Model;

namespace Viddy.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly ISettingsService _settingsService;

        public SettingsViewModel(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public bool IsLightTheme { get; set; }
        public bool IsLocationOn { get; set; }

        [UsedImplicitly]
        private void OnIsLightThemeChanged()
        {
            if (IsLightTheme)
            {
                ThemeManager.ToLightTheme();
                _settingsService.Theme = ElementTheme.Light;
            }
            else
            {
                ThemeManager.ToDarkTheme();
                _settingsService.Theme = ElementTheme.Dark;
            }
        }

        [UsedImplicitly]
        private void OnIsLocationOnChanged()
        {
            _settingsService.LocationIsOn = IsLocationOn;
        }

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
