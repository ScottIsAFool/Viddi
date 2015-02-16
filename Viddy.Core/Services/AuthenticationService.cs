using System;
using Cimbalino.Toolkit.Services;
using PropertyChanged;
using Viddy.Core.Extensions;
using VidMePortable;
using VidMePortable.Model;
using VidMePortable.Model.Responses;

namespace Viddy.Core.Services
{
    [ImplementPropertyChanged]
    public class AuthenticationService
    {
        private readonly IApplicationSettingsService _settingsService;
        private readonly IVidMeClient _vidMeClient;
        public static AuthenticationService Current { get; private set; }

        public event EventHandler UserSignedIn;
        public event EventHandler UserSignedOut;

        public AuthenticationService(IApplicationSettingsService settingsService, IVidMeClient vidMeClient)
        {
            _settingsService = settingsService;
            _vidMeClient = vidMeClient;
            _vidMeClient.AuthDetailsUpdated += VidMeClientOnAuthDetailsUpdated;
            Current = this;
        }

        private void VidMeClientOnAuthDetailsUpdated(object sender, AuthResponse authResponse)
        {
            SetAuthenticationInfo(authResponse);
        }

        public void StartService()
        {
            CheckForUser();

            var deviceId = Utils.UniqueDeviceIdentifier;
            _vidMeClient.SetDeviceNameAndPlatform(deviceId, "WindowsPhone");
            ServiceStarted = true;
        }

        public bool ServiceStarted { get; set; }

        public void SetAuthenticationInfo(AuthResponse authInfo, bool save = true)
        {
            AuthenticationInfo = authInfo;
            if (AuthenticationInfo == null
                || AuthenticationInfo.Auth == null)
            {
                return;
            }

            _vidMeClient.SetAuthentication(AuthenticationInfo.Auth);
            if (save)
            {
                _settingsService.Roaming.SetS(Constants.StorageSettings.AuthenticationSettings, AuthenticationInfo);
            }

            var signedIn = UserSignedIn;
            if (signedIn != null)
            {
                signedIn(this, EventArgs.Empty);
            }
        }

        public void SignOut()
        {
            _settingsService.Roaming.Remove(Constants.StorageSettings.AuthenticationSettings);
            AuthenticationInfo = null;

            var signedOut = UserSignedOut;
            if (signedOut != null)
            {
                signedOut(this, EventArgs.Empty);
            }
        }

        public IVidMeClient GetAuthenticatedVidMeClient()
        {
            return _vidMeClient;
        }

        public AuthResponse AuthenticationInfo { get; private set; }
        public bool IsLoggedIn { get { return AuthenticationInfo != null && AuthenticationInfo.Auth != null && AuthenticationInfo.User != null; } }
        public string LoggedInUserId { get { return IsLoggedIn ? AuthenticationInfo.User.UserId : "0"; } }
        public User LoggedInUser { get { return IsLoggedIn ? AuthenticationInfo.User : null; } }

        private void CheckForUser()
        {
            var auth = _settingsService.Roaming.GetS<AuthResponse>(Constants.StorageSettings.AuthenticationSettings);
            if (auth != null)
            {
                SetAuthenticationInfo(auth);
            }
        }
    }
}
