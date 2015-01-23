using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using Viddy.Views.Account;
using VidMePortable;

namespace Viddy.ViewModel
{
    public class ManageAccountViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IVidMeClient _vidMeClient;

        public ManageAccountViewModel(INavigationService navigationService, IVidMeClient vidMeClient, AvatarViewModel avatar)
        {
            Avatar = avatar;
            _navigationService = navigationService;
            _vidMeClient = vidMeClient;
        }

        public AvatarViewModel Avatar { get; set; }

        public RelayCommand ChangeAvatarCommand
        {
            get
            {
                return new RelayCommand(() => Avatar.ChangeAvatar());
            }
        }

        public RelayCommand NavigateToAppsAccessCommand
        {
            get
            {
                return new RelayCommand(() => _navigationService.Navigate<ManageAppsAccessView>());
            }
        }
    }
}
