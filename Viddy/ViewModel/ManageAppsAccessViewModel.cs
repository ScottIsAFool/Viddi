using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using Viddy.Common;
using Viddy.Services;
using Viddy.Views;
using VidMePortable;
using VidMePortable.Model;

namespace Viddy.ViewModel
{
    public class ManageAppsAccessViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IVidMeClient _vidMeClient;
        private readonly IMessageBoxService _messageBoxService;

        private bool _appsLoaded;

        public ManageAppsAccessViewModel(INavigationService navigationService, IVidMeClient vidMeClient, IMessageBoxService messageBoxService)
        {
            _navigationService = navigationService;
            _vidMeClient = vidMeClient;
            _messageBoxService = messageBoxService;

            if (IsInDesignMode)
            {
                Apps = new ObservableCollection<RevokeAppViewModel>
                {
                    new RevokeAppViewModel(new Application
                    {
                        Name = "Viddy for Windows Phone",
                        Organization = "Ferret Labs",
                        Website = "http://ferretlabs.com",
                        Description = "VidMe app for Windows Phone 8.1"
                    }, _vidMeClient, this, _messageBoxService)
                };
            }
        }

        public ObservableCollection<RevokeAppViewModel> Apps { get; set; }

        public RelayCommand PageLoadedCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await LoadData(false);
                });
            }
        }

        public RelayCommand RefreshCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await LoadData(true);
                });
            }
        }

        internal void ResetApp()
        {
            _appsLoaded = false;
            Apps = null;
            AuthenticationService.Current.SignOut();
            _navigationService.Navigate<VideoRecordView>(new NavigationParameters { ClearBackstack = true });
        }

        private async Task LoadData(bool isRefresh)
        {
            if (_appsLoaded && !isRefresh)
            {
                return;
            }

            SetProgressBar("Getting apps...");

            try
            {
                var response = await _vidMeClient.GetAuthorisedAppsAsync();

                Apps = new ObservableCollection<RevokeAppViewModel>(response.Select(x => new RevokeAppViewModel(x, _vidMeClient, this, _messageBoxService)));
                _appsLoaded = true;
            }
            catch (Exception ex)
            {
                
            }

            SetProgressBar();
        }
    }
}
