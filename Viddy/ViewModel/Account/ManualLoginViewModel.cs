using System;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using Viddy.Common;
using Viddy.Core.Services;
using Viddy.Views.Account;
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

        public string ErrorMessage { get; set; }

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
                    ErrorMessage = null;

                    try
                    {
                        SetProgressBar("Signing in...");
                        var response = await _vidMeClient.AuthenticateAsync(Username, Password);

                        AuthenticationService.Current.SetAuthenticationInfo(response);

                        if (_navigationService.CanGoBack)
                        {
                            _navigationService.GoBack();
                        }
                        else
                        {
                            _navigationService.Navigate<AccountView>(new NavigationParameters { RemoveCurrentPageFromBackstack = true });                            
                        }

                        Username = Password = string.Empty;
                    }
                    catch (VidMeException vex)
                    {
                        Log.ErrorException("SignInCommand", vex);
                        ErrorMessage = vex.Error != null && vex.Error.Code == "invalid_password" ? "Username and/or password incorrect" : "There was an error signing in";
                    }
                    catch (Exception ex)
                    {
                        Log.ErrorException("SignInCommand", ex);
                        ErrorMessage = "There was an error signing in";
                    }

                    SetProgressBar();
                }, () => CanSignIn);
            }
        }

        public RelayCommand NavigateToCreateAccountCommand
        {
            get
            {
                return new RelayCommand(() => _navigationService.Navigate<CreateAccountView>());
            }
        }

        public override void UpdateProperties()
        {
            base.UpdateProperties();
            RaisePropertyChanged(() => CanSignIn);
        }
    }
}
