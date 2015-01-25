using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Messaging;
using Viddy.Messaging;
using VidMePortable.Model;

namespace Viddy.ViewModel.Account
{
    public class ProfileViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        public ProfileViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            if (IsInDesignMode)
            {
                // TODO: Need to find a less NSFW profile to use
                User = new User
                {
                    Username = "MarinaSweet",
                    AvatarUrl = "https://d1wst0behutosd.cloudfront.net/avatars/10042.gif?gv2r1421446675",
                    CoverUrl = "https://d1wst0behutosd.cloudfront.net/channel_covers/10042.jpg?v1r1420500373",
                    FollowerCount = 120,
                    LikesCount = "92",
                    VideoCount = 532,
                    VideoViews = "71556",
                    VideosScores = 220,
                    Bio = "Some bio information"
                };
            }
        }

        public User User { get; set; }

        protected override void WireMessages()
        {
            Messenger.Default.Register<UserMessage>(this, m =>
            {
                User = m.User;
            });
        }
    }
}
