using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Viddy.Extensions;
using Viddy.Messaging;
using VidMePortable;
using VidMePortable.Model;

namespace Viddy.ViewModel.Account
{
    public class ProfileViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IVidMeClient _vidMeClient;

        private bool _videosLoaded;

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
            }
        }

        public User User { get; set; }
        public ObservableCollection<VideoItemViewModel> Videos { get; set; }
        public bool IsEmpty { get; set; }
        public bool IsLoadingMore { get; set; }
        public bool CanLoadMore { get; set; }

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

        public RelayCommand PageLoadedCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await LoadData(false, false, 0);
                });
            }
        }

        public RelayCommand RefreshCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await LoadData(true, false, 0);
                });
            }
        }

        public RelayCommand LoadMoreCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await LoadData(false, true, Videos.Count);
                });
            }
        }

        private async Task LoadData(bool isRefresh, bool add, int offset)
        {
            if (_videosLoaded && !isRefresh && !add)
            {
                return;
            }

            try
            {
                if (!add)
                {
                    SetProgressBar("Getting videos...");
                }

                IsLoadingMore = add;
                var response = await _vidMeClient.GetUserVideosAsync(User.UserId, offset);

                if (Videos == null || !add)
                {
                    Videos = new ObservableCollection<VideoItemViewModel>();
                }

                foreach (var video in response.Videos.Select(x => new VideoItemViewModel(x)))
                {
                    Videos.Add(video);
                }

                IsEmpty = Videos.IsNullOrEmpty();
                CanLoadMore = Videos.Count <= response.Page.Total;
                _videosLoaded = true;
            }
            catch (Exception ex)
            {
                
            }

            IsLoadingMore = false;
            SetProgressBar();
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
