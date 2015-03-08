using System;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using Viddi.Common;
using Viddi.Core.Services;
using Viddi.Localisation;
using Viddi.Views.Account;
using VidMePortable;
using VidMePortable.Model;

namespace Viddi.ViewModel.Account
{
    public class CreateAccountViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IVidMeClient _vidMeClient;

        public CreateAccountViewModel(INavigationService navigationService, IVidMeClient vidMeClient)
        {
            _navigationService = navigationService;
            _vidMeClient = vidMeClient;
        }

        public string Username { get; set; }
        public string Password { get; set; }
        public string EmailAddress { get; set; }

        public string ErrorMessage { get; set; }

        public bool CanCreateAccount
        {
            get
            {
                return !ProgressIsVisible
                       && !string.IsNullOrEmpty(Username)
                       && !string.IsNullOrEmpty(Password);
            }
        }

        public RelayCommand CreateAccountCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    try
                    {
                        ErrorMessage = null;
                        SetProgressBar("Creating user...");

                        var response = await _vidMeClient.CreateUserAsync(Username, Password, EmailAddress);

                        AuthenticationService.Current.SetAuthenticationInfo(response);

                        _navigationService.Navigate<AccountView>(new NavigationParameters { ClearBackstack = true });

                        Username = Password = EmailAddress = string.Empty;
                    }
                    catch (VidMeException vex)
                    {
                        Log.ErrorException("CreateAccountCommand(vex)", vex);
                        ErrorMessage = vex.Error != null && vex.Error.Code == "used_username" ? Resources.ErrorSignupUsernameExists : Resources.ErrorSignupGeneric;
                    }
                    catch (Exception ex)
                    {
                        Log.ErrorException("CreateAccountCommand(ex)", ex);
                        ErrorMessage = Resources.ErrorSignupGeneric;
                    }

                    SetProgressBar();
                }, () => CanCreateAccount);
            }
        }

        public override void UpdateProperties()
        {
            base.UpdateProperties();
            RaisePropertyChanged(() => CanCreateAccount);
        }
    }
}
