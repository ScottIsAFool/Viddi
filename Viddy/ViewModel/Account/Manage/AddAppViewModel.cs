using System;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using VidMePortable;
using VidMePortable.Model;
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
                        var app = new AppRequest
                        {
                            Name = Name,
                            Description = Description,
                            Organisation = Organisation,
                            RedirectUri = RedirectUrl,
                            Website = Website
                        };

                        var response = await _vidMeClient.RegisterAppAsync(app);
                        
                    }
                    catch (Exception ex)
                    {
                        
                    }
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