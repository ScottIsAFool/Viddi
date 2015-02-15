using System;
using Cimbalino.Toolkit.Services;
using Coding4Fun.Toolkit.Controls;
using GalaSoft.MvvmLight.Command;
using Viddy.Core.Services;
using Viddy.Services;
using Viddy.ViewModel.Item;
using VidMePortable;
using VidMePortable.Model;

namespace Viddy.ViewModel.Account
{
    public class EditProfileViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IVidMeClient _vidMeClient;
        private readonly IToastService _toastService;

        public EditProfileViewModel(INavigationService navigationService, IVidMeClient vidMeClient, AvatarViewModel avatarViewModel, IToastService toastService)
        {
            Avatar = avatarViewModel;
            _navigationService = navigationService;
            _vidMeClient = vidMeClient;
            _toastService = toastService;
        }

        public AvatarViewModel Avatar { get; set; }

        public string Name { get; set; }
        public string Bio { get; set; }
        public string Email { get; set; }

        public string NewPassword { get; set; }
        public string CurrentPassword { get; set; }
        public bool IsChanged { get; set; }

        public bool CanUpdate
        {
            get
            {
                return !ProgressIsVisible
                       && IsChanged
                       && !string.IsNullOrEmpty(Name)
                       && (string.IsNullOrEmpty(NewPassword) && string.IsNullOrEmpty(CurrentPassword)
                           || (!string.IsNullOrEmpty(NewPassword) && !string.IsNullOrEmpty(CurrentPassword)));
            }
        }

        public RelayCommand UpdateProfileCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    try
                    {
                        SetProgressBar("Updating profile...");

                        string newPassword = null, currentPassword = null;
                        if (!string.IsNullOrEmpty(NewPassword) && !string.IsNullOrEmpty(CurrentPassword))
                        {
                            newPassword = NewPassword;
                            currentPassword = CurrentPassword;
                        }

                        var response = await _vidMeClient.EditUserAsync(AuthenticationService.Current.LoggedInUserId, Name, currentPassword, newPassword, Email, Bio);
                        if (response != null)
                        {
                            var auth = AuthenticationService.Current.AuthenticationInfo;
                            auth.User = response;
                            AuthenticationService.Current.SetAuthenticationInfo(auth);
                        }

                        NewPassword = CurrentPassword = string.Empty;

                        _toastService.Show("Changes saved");
                    }
                    catch (VidMeException ex)
                    {
                        if (ex.Error != null && !string.IsNullOrEmpty(ex.Error.Error))
                        {
                            _toastService.Show(ex.Error.Error);
                        }
                    }

                    SetProgressBar();
                }, () => CanUpdate);
            }
        }

        public RelayCommand PageLoadedCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    var user = AuthenticationService.Current.AuthenticationInfo.User;
                    Name = user.Username;
                    Bio = user.Bio;
                    Email = user.Email;

                    IsChanged = false;
                }, () => AuthenticationService.Current.IsLoggedIn);
            }
        }

        public override void UpdateProperties()
        {
            RaisePropertyChanged(() => CanUpdate);
        }
    }
}
