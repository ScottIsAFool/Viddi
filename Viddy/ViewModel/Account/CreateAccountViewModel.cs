using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using Viddy.Common;
using Viddy.Services;
using Viddy.Views.Account;
using VidMePortable;
using VidMePortable.Model;

namespace Viddy.ViewModel.Account
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
                    if (!CanCreateAccount)
                    {
                        return;
                    }

                    try
                    {
                        SetProgressBar("Creating user...");

                        var response = await _vidMeClient.CreateUserAsync(Username, Password, EmailAddress);

                        AuthenticationService.Current.SetAuthenticationInfo(response);

                        _navigationService.Navigate<AccountView>(new NavigationParameters {ClearBackstack = true});

                        Username = Password = EmailAddress = string.Empty;
                    }
                    catch (VidMeException ex)
                    {
                        if (ex.Error.Code == "used_username")
                        {
                            // TODO: Display an error 
                        }
                    }

                    SetProgressBar();
                });
            }
        }

        public override void UpdateProperties()
        {
            base.UpdateProperties();
            RaisePropertyChanged(() => CanCreateAccount);
        }
    }
}
