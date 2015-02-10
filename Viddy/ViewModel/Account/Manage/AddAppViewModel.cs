using System;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Viddy.Common;
using Viddy.Views.Account.Manage;
using VidMePortable;
using VidMePortable.Model.Requests;

namespace Viddy.ViewModel.Account.Manage
{
    public class AddAppViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IVidMeClient _vidMeClient;

        public AddAppViewModel(INavigationService navigationService, IVidMeClient vidMeClient)
        {
            _navigationService = navigationService;
            _vidMeClient = vidMeClient;
        }

        public string Name { get; set; }
        public string Website { get; set; }
        public string Description { get; set; }
        public string Organisation { get; set; }
        public string RedirectUrl { get; set; }

        public bool CanAddApp
        {
            get
            {
                return !ProgressIsVisible
                       && !string.IsNullOrEmpty(Name)
                       && !string.IsNullOrEmpty(RedirectUrl);
            }
        }

        public RelayCommand SaveAppCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    try
                    {
                        SetProgressBar("Adding app...");
                        var app = new AppRequest
                        {
                            Name = Name,
                            Description = Description,
                            Organisation = Organisation,
                            RedirectUri = RedirectUrl,
                            Website = Website
                        };

                        var response = await _vidMeClient.RegisterAppAsync(app);
                        if (response != null)
                        {
                            Messenger.Default.Send(new NotificationMessage(Constants.Messages.NewAppAddedMsg));
                            if (_navigationService.CanGoBack)
                            {
                                _navigationService.GoBack();
                            }
                            else
                            {
                                _navigationService.Navigate<ManageMyAppsView>(new NavigationParameters {ClearBackstack = true});
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        
                    }

                    SetProgressBar();
                }, () => CanAddApp);
            }
        }

        public override void UpdateProperties()
        {
            base.UpdateProperties();
            RaisePropertyChanged(() => CanAddApp);
        }
    }
}