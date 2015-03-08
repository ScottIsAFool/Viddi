using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using Viddi.Views.Account.Manage;

namespace Viddi.ViewModel.Account.Manage
{
    public class ManageAccountViewModel
    {
        private readonly INavigationService _navigationService;

        public ManageAccountViewModel(INavigationService navigationService, AvatarViewModel avatar)
        {
            Avatar = avatar;
            _navigationService = navigationService;
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

        public RelayCommand NavigateToMyAppsCommand
        {
            get
            {
                return new RelayCommand(() => _navigationService.Navigate<ManageMyAppsView>());
            }
        }
    }
}
