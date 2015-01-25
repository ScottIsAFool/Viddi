using System;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using Viddy.Services;
using VidMePortable;
using VidMePortable.Model;

namespace Viddy.ViewModel.Account
{
    public class ManualLoginViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IVidMeClient _vidMeClient;

        public ManualLoginViewModel(INavigationService navigationService, IVidMeClient vidMeClient)
        {
            _navigationService = navigationService;
            _vidMeClient = vidMeClient;
        }

        public string Username { get; set; }
        public string Password { get; set; }

        public bool CanSignIn
        {
            get
            {
                return !string.IsNullOrEmpty(Username)
                       && !string.IsNullOrEmpty(Password)
                       && !ProgressIsVisible;
            }
        }

        public RelayCommand SignInCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    if (!CanSignIn)
                    {
                        return;
                    }

                    try
                    {
                        SetProgressBar("Signing in...");
                        var response = await _vidMeClient.AuthenticateAsync(Username, Password);

                        AuthenticationService.Current.SetAuthenticationInfo(response);
                    }
                    catch (VidMeException vex)
                    {
                        if (vex.Error.Code == "invalid_password")
                        {
                            // TODO: Display error message
                        }
                    }
                    catch (Exception ex)
                    {

                    }


                    SetProgressBar();
                });
            }
        }

        public RelayCommand NavigateToCreateAccountCommand
        {
            get
            {
                return new RelayCommand(() =>
                {

                });
            }
        }

        public override void UpdateProperties()
        {
            base.UpdateProperties();
            RaisePropertyChanged(() => CanSignIn);
        }
    }
}
