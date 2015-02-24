using System;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using Viddy.Common;
using Viddy.Core.Services;
using Viddy.Views.Account;
using VidMePortable;
using VidMePortable.Model;
using VidMePortable.Model.Responses;

namespace Viddy.ViewModel.Account
{
    public class CreateAccountViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IVidMeClient _vidMeClient;
        private readonly ILocalisationLoader _localisationLoader;

        public CreateAccountViewModel(INavigationService navigationService, IVidMeClient vidMeClient, ILocalisationLoader localisationLoader)
        {
            _navigationService = navigationService;
            _vidMeClient = vidMeClient;
            _localisationLoader = localisationLoader;
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
                        ErrorMessage = vex.Error != null && vex.Error.Code == "used_username" ? _localisationLoader.GetString("ErrorSignupUsernameExists") : _localisationLoader.GetString("ErrorSignupGeneric");
                    }
                    catch (Exception ex)
                    {
                        Log.ErrorException("CreateAccountCommand(ex)", ex);
                        ErrorMessage = _localisationLoader.GetString("ErrorSignupGeneric");
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
