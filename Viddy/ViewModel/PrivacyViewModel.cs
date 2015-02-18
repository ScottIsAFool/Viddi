using System;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;

namespace Viddy.ViewModel
{
    public class PrivacyViewModel : ViewModelBase
    {
        private readonly ILauncherService _launcherService;

        public PrivacyViewModel(ILauncherService launcherService)
        {
            _launcherService = launcherService;
        }

        public RelayCommand PrivacyCommand
        {
            get
            {
                return new RelayCommand(() => _launcherService.LaunchUriAsync(new Uri("https://vid.me/privacy", UriKind.Absolute)));
            }
        }
    }
}
