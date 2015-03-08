using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using JetBrains.Annotations;
using ThemeManagerRt;
using Viddi.Core.Extensions;
using Viddi.Model;
using Viddi.Services;
using Viddi.Views;

namespace Viddi.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly ISettingsService _settingsService;
        private readonly ITaskService _taskService;

        private bool _ignoreChanges;

        public SettingsViewModel(INavigationService navigationService, ISettingsService settingsService, ITaskService taskService)
        {
            _navigationService = navigationService;
            _settingsService = settingsService;
            _taskService = taskService;

            _ignoreChanges = true;
            UpdateFrequencies = Enum.GetValues(typeof(UpdateFrequency)).ToList<UpdateFrequency>();
            NotificationFrequency = UpdateFrequencies.First(x => x == _settingsService.NotificationFrequency.GetFrequency());
            IsLightTheme = _settingsService.Theme == ElementTheme.Light;
            _ignoreChanges = false;
        }

        public bool IsLightTheme { get; set; }
        public bool IsLocationOn { get; set; }
        public bool CheckForNotificationsInBackground { get; set; }
        public UpdateFrequency NotificationFrequency { get; set; }
        public List<UpdateFrequency> UpdateFrequencies { get; set; }

        public RelayCommand NavigateToPrivacyCommand
        {
            get { return new RelayCommand(() => _navigationService.Navigate<PrivacyView>()); }
        }

        [UsedImplicitly]
        private void OnCheckForNotificationsInBackgroundChanged()
        {
            if (_ignoreChanges) return;

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
        private void OnNotificationFrequencyChanged()
        {
            if (_ignoreChanges) return;

            _taskService.RemoveService();
            _taskService.CreateService(NotificationFrequency.GetMinutes());

            _settingsService.NotificationFrequency = NotificationFrequency.GetMinutes();
        }

        [UsedImplicitly]
        private void OnIsLightThemeChanged()
        {
            if (_ignoreChanges) return;

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
