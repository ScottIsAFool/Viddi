using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Security.Authentication.Web;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Viddy.Services;
using VidMePortable;
using VidMePortable.Model;

namespace Viddy.ViewModel
{
    public class AccountViewModel : ViewModelBase
    {
        private const string DefaultAvatar = "/Assets/Defaults/UserLoginDefault.png";

        private readonly INavigationService _navigationService;
        private readonly IVidMeClient _vidMeClient;

        public AccountViewModel(INavigationService navigationService, IVidMeClient vidMeClient)
        {
            _navigationService = navigationService;
            _vidMeClient = vidMeClient;

            SetAvatar();
        }

        public RelayCommand LogInLogOutCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (AuthenticationService.Current.IsLoggedIn)
                    {
                        AuthenticationService.Current.SignOut();
                        SetAvatar();
                    }
                    else
                    {
                        LaunchAuthentication();
                    }
                });
            }
        }

        private void SetAvatar()
        {
            AvatarUrl = AuthenticationService.Current.IsLoggedIn ? AuthenticationService.Current.AuthenticationInfo.User.AvatarUrl : DefaultAvatar;
        }

        public string AvatarUrl { get; set; }

        private void LaunchAuthentication()
        {
            var url = _vidMeClient.GetAuthUrl(Constants.ClientId, Constants.CallBackUrl, new List<Scope> {Scope.Videos, Scope.VideoUpload, Scope.Channels, Scope.Comments, Scope.Votes, Scope.Account});

            WebAuthenticationBroker.AuthenticateAndContinue(new Uri(url), new Uri(Constants.CallBackUrl), new ValueSet(), WebAuthenticationOptions.None);
        }

        private async Task CompleteAuthentication(string code)
        {
            var auth = await _vidMeClient.ExchangeCodeForTokenAsync(code, Constants.ClientId, Constants.ClientSecret);
            if (auth != null)
            {
                AuthenticationService.Current.SetAuthenticationInfo(auth);
            }
            
            SetAvatar();
        }

        protected override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, async m =>
            {
                if (m.Notification.Equals(Constants.Messages.AuthCodeMsg))
                {
                    var code = (string) m.Sender;
                    await CompleteAuthentication(code);
                }
            });
        }
    }
}
