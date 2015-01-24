using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using Viddy.Views.Account.Manage;
using VidMePortable;
using VidMePortable.Model;

namespace Viddy.ViewModel.Account.Manage
{
    public class ManageMyAppsViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IVidMeClient _vidMeClient;

        private bool _appsLoaded;

        public ManageMyAppsViewModel(INavigationService navigationService, IVidMeClient vidMeClient)
        {
            _navigationService = navigationService;
            _vidMeClient = vidMeClient;

            if (IsInDesignMode)
            {
                Apps = new ObservableCollection<OwnedAppViewModel>
                {
                    new OwnedAppViewModel(new Application
                    {
                        ClientId = "kjsdlfkjlsdkfjlskdjf09wefj0w9e",
                        Name = "Viddy for Windows Phone"
                    }, _vidMeClient)
                };
            }
        }

        public ObservableCollection<OwnedAppViewModel> Apps { get; set; }

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

        public RelayCommand AddAppCommand
        {
            get
            {
                return new RelayCommand(()=> _navigationService.Navigate<AddAppView>());
            }
        }

        private async Task LoadData(bool isRefresh)
        {
            if (_appsLoaded && !isRefresh)
            {
                return;
            }

            try
            {
                SetProgressBar("Getting apps...");

                var response = await _vidMeClient.GetOwnedAppsAsync();

                Apps = new ObservableCollection<OwnedAppViewModel>(response.Select(x => new OwnedAppViewModel(x, _vidMeClient)));

                _appsLoaded = true;
            }
            catch (Exception ex)
            {
                
            }

            SetProgressBar();
        }
    }
}
