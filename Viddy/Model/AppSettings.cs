using Windows.UI.Xaml;
using Cimbalino.Toolkit.Services;
using JetBrains.Annotations;
using PropertyChanged;
using ThemeManagerRt;
using Viddy.Core;
using Viddy.Core.Extensions;

namespace Viddy.Model
{
    public interface ISettingsService
    {
        void StartService();
        ElementTheme Theme { get; set; }
        bool LocationIsOn { get; set; }
        bool CheckForNotificationsInBackground { get; set; }
        int NotificationFrequency { get; set; }
        void Save();
    }

    [ImplementPropertyChanged]
    public class SettingsService : ISettingsService
    {
        private readonly IApplicationSettingsService _applicationSettings;

        private bool _serviceStarted;

        public SettingsService(IApplicationSettingsService applicationSettings)
        {
            _applicationSettings = applicationSettings;
            Theme = ElementTheme.Light;
            NotificationFrequency = 30;
        }

        public void StartService()
        {
            LoadSettings();
            //ThemeManager.Th = Theme;
            var themeManager = Application.Current.Resources["ThemeManager"] as ThemeManager;
            if (Theme == ElementTheme.Light)
            {
                ThemeManager.ToLightTheme();
            }
            else
            {
                ThemeManager.ToLightTheme();
            }
            _serviceStarted = true;
        }

        private void LoadSettings()
        {
            var settings = _applicationSettings.Roaming.GetS<SettingsService>(Constants.StorageSettings.ApplicationSettings);
            if (settings != null)
            {
                Theme = settings.Theme;
                LocationIsOn = settings.LocationIsOn;
            }
        }

        public ElementTheme Theme { get; set; }
        public bool LocationIsOn { get; set; }
        public bool CheckForNotificationsInBackground { get; set; }
        public int NotificationFrequency { get; set; }

        [UsedImplicitly]
        private void OnCheckForNotificationsInBackgroundChanged()
        {
            Save();
        }

        [UsedImplicitly]
        private void OnNotificationCheckFrequencyInMinutesChanged()
        {
            Save();
        }

        [UsedImplicitly]
        private void OnThemeChanged()
        {
            Save();
        }

        [UsedImplicitly]
        private void OnLocationIsOnChanged()
        {
            Save();
        }

        public void Save()
        {
            if (!_serviceStarted) return;
            _applicationSettings.Roaming.SetS(Constants.StorageSettings.ApplicationSettings, this);
        }
    }
}
