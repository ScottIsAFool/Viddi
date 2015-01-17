using Cimbalino.Toolkit.Services;
using PropertyChanged;
using Viddy.Extensions;
using VidMePortable.Model.Responses;

namespace Viddy.Services
{
    [ImplementPropertyChanged]
    public class AuthenticationService
    {
        private readonly IApplicationSettingsService _settingsService;
        public static AuthenticationService Current { get; private set; }

        public AuthenticationService(IApplicationSettingsService settingsService)
        {
            _settingsService = settingsService;
            Current = this;
        }

        public void StartService()
        {
            CheckForUser();
        }

        public void SetAuthenticationInfo(AuthResponse authInfo, bool save = true)
        {
            AuthenticationInfo = authInfo;
            if (save)
            {
                _settingsService.Roaming.SetS(Constants.StorageSettings.AuthenticationSettings, AuthenticationInfo);
            }
        }

        public void SignOut()
        {
            _settingsService.Roaming.Remove(Constants.StorageSettings.AuthenticationSettings);
            AuthenticationInfo = null;
        }

        public AuthResponse AuthenticationInfo { get; private set; }
        public bool IsLoggedIn { get { return AuthenticationInfo != null && AuthenticationInfo.Auth != null && AuthenticationInfo.User != null; } }
        public string LoggedInUserId { get { return IsLoggedIn ? AuthenticationInfo.User.UserId : "0"; } }

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
