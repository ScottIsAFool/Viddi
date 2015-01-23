using Cimbalino.Toolkit.Services;
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

    }
}
