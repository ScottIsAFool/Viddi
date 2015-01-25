using System.Threading.Tasks;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Messaging;
using Viddy.Messaging;
using VidMePortable;
using VidMePortable.Model;
using VidMePortable.Model.Responses;

namespace Viddy.ViewModel.Account
{
    public class ProfileViewModel : VideoLoadingViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IVidMeClient _vidMeClient;

        public ProfileViewModel(INavigationService navigationService, IVidMeClient vidMeClient)
        {
            _navigationService = navigationService;
            _vidMeClient = vidMeClient;

            if (IsInDesignMode)
            {
                User = new UserViewModel(new User
                {
                    UserId = "59739",
                    Username = "PunkHack",
                    AvatarUrl = "https://d1wst0behutosd.cloudfront.net/avatars/59739.gif?gv2r1420954820",
                    CoverUrl = "https://d1wst0behutosd.cloudfront.net/channel_covers/59739.jpg?v1r1420500373",
                    FollowerCount = 1200,
                    LikesCount = "92",
                    VideoCount = 532,
                    VideoViews = "71556",
                    VideosScores = 220,
                    Bio = "Some bio information"
                });
                IsEmpty = true;
            }
        }

        public UserViewModel User { get; set; }

        public UserViewModel EmptyUser
        {
            get
            {
                return new UserViewModel(new User());
            }
        }


        public override Task<VideosResponse> GetVideos(int offset)
        {
            return _vidMeClient.GetUserVideosAsync(User.User.UserId, offset);
        }

        protected override void WireMessages()
        {
            Messenger.Default.Register<UserMessage>(this, m =>
            {
                User = new UserViewModel(m.User);
            });
        }
    }
}
