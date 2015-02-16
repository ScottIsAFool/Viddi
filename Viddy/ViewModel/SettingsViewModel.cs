using Windows.UI.Xaml;
using GalaSoft.MvvmLight.Command;
using JetBrains.Annotations;
using ThemeManagerRt;
using Viddy.Model;
using Viddy.Services;

namespace Viddy.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly ISettingsService _settingsService;
        private readonly ITaskService _taskService;

        public SettingsViewModel(ISettingsService settingsService, ITaskService taskService)
        {
            _settingsService = settingsService;
            _taskService = taskService;
        }

        public bool IsLightTheme { get; set; }
        public bool IsLocationOn { get; set; }
        public bool CheckForNotificationsInBackground { get; set; }
        public int NotificationCheckFrequencyInMinutes { get; set; }

        [UsedImplicitly]
        private void OnCheckForNotificationsInBackgroundChanged()
        {
            if (CheckForNotificationsInBackground)
            {
                _taskService.CreateService();
            }
            else
            {
                _taskService.RemoveService();
            }

            _settingsService.CheckForNotificationsInBackground = CheckForNotificationsInBackground;
        }

        [UsedImplicitly]
        private void OnNotificationCheckFrequencyInMinutesChanged()
        {
            _taskService.RemoveService();
            _taskService.CreateService(NotificationCheckFrequencyInMinutes);

            _settingsService.NotificationCheckFrequencyInMinutes = NotificationCheckFrequencyInMinutes;
        }

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
    }
}
