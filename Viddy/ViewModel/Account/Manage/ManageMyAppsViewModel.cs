using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Viddy.Core;
using Viddy.Core.Extensions;
using Viddy.Extensions;
using Viddy.Views.Account.Manage;
using VidMePortable;
using VidMePortable.Model;

namespace Viddy.ViewModel.Account.Manage
{
    public class ManageMyAppsViewModel : LoadingItemsViewModel<OwnedAppViewModel>
    {
        private readonly INavigationService _navigationService;
        private readonly IVidMeClient _vidMeClient;

        public ManageMyAppsViewModel(INavigationService navigationService, IVidMeClient vidMeClient)
        {
            _navigationService = navigationService;
            _vidMeClient = vidMeClient;

            if (IsInDesignMode)
            {
                Items = new ObservableCollection<OwnedAppViewModel>
                {
                    new OwnedAppViewModel(new Application
                    {
                        ClientId = "kjsdlfkjlsdkfjlskdjf09wefj0w9e",
                        Name = "Viddy for Windows Phone"
                    }, _vidMeClient)
                };

                IsEmpty = true;
            }
        }

        public RelayCommand AddAppCommand
        {
            get
            {
                return new RelayCommand(()=> _navigationService.Navigate<AddAppView>());
            }
        }

        protected override async Task LoadData(bool isRefresh, bool add = false, int offset = 0)
        {
            if (ItemsLoaded && !isRefresh)
            {
                return;
            }

            HasErrors = false;

            try
            {
                SetProgressBar("Getting apps...");

                var response = await _vidMeClient.GetOwnedAppsAsync();

                Items = new ObservableCollection<OwnedAppViewModel>(response.Select(x => new OwnedAppViewModel(x, _vidMeClient)));
                IsEmpty = Items.IsNullOrEmpty();

                ItemsLoaded = true;
            }
            catch (Exception ex)
            {
                Log.ErrorException("LoadData()", ex);
                HasErrors = true;
            }

            SetProgressBar();
        }

        protected override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, m =>
            {
                if (m.Notification.Equals(Constants.Messages.NewAppAddedMsg))
                {
                    ItemsLoaded = false;
                }
            });
        }
    }
}
