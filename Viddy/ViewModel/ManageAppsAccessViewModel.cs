using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
                Apps = new ObservableCollection<Application>
                {
                    new Application
                    {
                        Name = "Viddy for Windows Phone",
                        Organization = "Ferret Labs",
                        Website = "http://ferretlabs.com",
                        Description = "VidMe app for Windows Phone 8.1"
                    }
                };
            }
        }

        public ObservableCollection<Application> Apps { get; set; }

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

        public RelayCommand<Application> RevokeTokenCommand
        {
            get
            {
                return new RelayCommand<Application>(async app =>
                {
                    try
                    {
                        if (app.ClientId == Constants.ClientId)
                        {
                            var result = await _messageBoxService.ShowAsync("This is the token for this app, are you sure you wish to revoke this token? If you do, you will have to sign in again.", "Are you sure?", new List<string> {"yes", "no"});
                            if (result == 1)
                            {
                                return;
                            }
                        }

                        SetProgressBar("Revoking token...");
                        if (await _vidMeClient.RevokeAppTokenAsync(app.ClientId))
                        {
                            if (app.ClientId == Constants.ClientId)
                            {
                                _appsLoaded = false;
                                Apps = null;
                                AuthenticationService.Current.SignOut();
                                _navigationService.Navigate<VideoRecordView>(new NavigationParameters {ClearBackstack = true});
                            }
                            else
                            {
                                Apps.Remove(app);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }

                    SetProgressBar();
                });
            }
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

                Apps = new ObservableCollection<Application>(response);
                _appsLoaded = true;
            }
            catch (Exception ex)
            {
                
            }

            SetProgressBar();
        }
    }
}
