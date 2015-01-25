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
                User = new User
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
                };

                IsEmpty = true;
            }
        }

        public User User { get; set; }

        public string UserFollowers
        {
            get { return User != null && User.FollowerCount > 0 ? string.Format("{0:N0} followers", User.FollowerCount) : null; }
        }

        public string UserPlays
        {
            get
            {
                double views;
                return User != null
                       && !string.IsNullOrEmpty(User.VideoViews)
                       && double.TryParse(User.VideoViews, out views)
                       && views > 0
                    ? string.Format("{0:N0} plays", views)
                    : null;
            }
        }

        public string UserVideoCount
        {
            get { return User != null && User.VideoCount > 0 ? string.Format("{0:N0} videos", User.VideoCount) : null; }
        }

        public override Task<VideosResponse> GetVideos(int offset)
        {
            return _vidMeClient.GetUserVideosAsync(User.UserId, offset);
        }

        protected override void WireMessages()
        {
            Messenger.Default.Register<UserMessage>(this, m =>
            {
                User = m.User;
            });
        }
    }
}
