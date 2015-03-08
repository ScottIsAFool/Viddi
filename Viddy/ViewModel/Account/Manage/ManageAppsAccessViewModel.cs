using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Cimbalino.Toolkit.Services;
using ScottIsAFool.Windows.Helpers;
using Viddi.Core.Services;
using Viddi.Views;
using VidMePortable;
using VidMePortable.Model;

namespace Viddi.ViewModel.Account.Manage
{
    public class ManageAppsAccessViewModel : LoadingItemsViewModel<RevokeAppViewModel>
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
                Items = new ObservableCollection<RevokeAppViewModel>
                {
                    new RevokeAppViewModel(new Application
                    {
                        Name = "Viddi for Windows Phone",
                        Organization = "Ferret Labs",
                        Website = "http://ferretlabs.com",
                        Description = "VidMe app for Windows Phone 8.1"
                    }, _vidMeClient, this, _messageBoxService)
                };
            }
        }

        internal void ResetApp()
        {
            _appsLoaded = false;
            Items = null;
            AuthenticationService.Current.SignOut();
            _navigationService.Navigate<VideoRecordView>(new NavigationParameters { ClearBackstack = true });
        }

        protected override async Task LoadData(bool isRefresh, bool add = false, int offset = 0)
        {
            if (_appsLoaded && !isRefresh)
            {
                return;
            }

            HasErrors = false;

            SetProgressBar("Getting apps...");

            try
            {
                var response = await _vidMeClient.GetAuthorisedAppsAsync();

                Items = new ObservableCollection<RevokeAppViewModel>(response.Select(x => new RevokeAppViewModel(x, _vidMeClient, this, _messageBoxService)));

                _appsLoaded = true;
            }
            catch (Exception ex)
            {
                Log.ErrorException("LoadData()", ex);
                HasErrors = true;
            }

            SetProgressBar();
        }
    }
}
