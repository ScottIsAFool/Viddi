using Windows.UI.Xaml;
using Cimbalino.Toolkit.Services;
using JetBrains.Annotations;
using PropertyChanged;
using ThemeManagerRt;
using Viddy.Extensions;

namespace Viddy.Model
{
    public interface ISettingsService
    {
        void StartService();
        ElementTheme Theme { get; set; }
        bool LocationIsOn { get; set; }
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
        }

        public void StartService()
        {
            LoadSettings();
            ThemeManager.DefaultTheme = Theme;
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
