using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Cimbalino.Toolkit.Extensions;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Viddy.Extensions;
using Viddy.Messaging;
using Viddy.Model;
using Viddy.Services;
using Viddy.Views;
using VidMePortable;
using VidMePortable.Model;

namespace Viddy.ViewModel
{
    public class VideoItemViewModel : ViewModelBase, IListType
    {
        private readonly IVidMeClient _vidMeClient;
        private readonly VideoLoadingViewModel _videoLoadingViewModel;
        private readonly IApplicationSettingsService _settingsService;
        private readonly INavigationService _navigationService;

        private bool _commentsLoaded;

        public VideoItemViewModel(Video video, VideoLoadingViewModel videoLoadingViewModel)
        {
            _vidMeClient = SimpleIoc.Default.GetInstance<IVidMeClient>();
            _settingsService = SimpleIoc.Default.GetInstance<IApplicationSettingsService>();
            _navigationService = SimpleIoc.Default.GetInstance<INavigationService>();
            _videoLoadingViewModel = videoLoadingViewModel;
            Video = video;

            if (IsInDesignMode)
            {
                Comments = new ObservableCollection<CommentViewModel>
                {
                    new CommentViewModel(new Comment
                    {
                        Body = "This is a comment",
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
                        }
                    }, this)
                };

                Video = new Video
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
                    }
                };
            }
        }

        public Video Video { get; set; }

        public ObservableCollection<CommentViewModel> Comments { get; set; }
        public bool IsEmpty { get; set; }
        public bool CanLoadMore { get; set; }
        public bool IsLoadingMore { get; set; }

        public string Title
        {
            get { return Video != null && !string.IsNullOrEmpty(Video.Title) ? Video.Title : "Untitled"; }
        }

        public bool CanDelete
        {
            get
            {
                return Video != null
                       && (AuthenticationService.Current.IsLoggedIn
                       && AuthenticationService.Current.LoggedInUserId == Video.UserId)
                       || VideoIsAnonymousButOwned();
            }
        }

        public string SubmittedBy
        {
            get
            {
                if (Video == null)
                {
                    return string.Empty;
                }

                return IsAnonymous ? "Anonymous" : Video.User.Username;
            }
        }

        public bool IsAnonymous
        {
            get { return Video == null || string.IsNullOrEmpty(Video.UserId) || Video.User == null; }
        }

        public bool DisplayDuration
        {
            get { return Video != null && Video.Duration.HasValue; }
        }

        public string VideoLength
        {
            get
            {
                if (IsInDesignMode)
                {
                    return "0:07";
                }

                if (Video == null || !Video.Duration.HasValue)
                {
                    return string.Empty;
                }

                var ts = TimeSpan.FromSeconds(Video.Duration.Value);
                if (ts.TotalHours > 1)
                {
                    return string.Format("{0:c}", ts);
                }

                return string.Format("{0:0}:{1:00}", ts.Minutes, ts.Seconds);
            }
        }

        public int CommentCount
        {
            get { return Video != null ? Video.CommentCount : 0; }
        }

        public RelayCommand DeleteCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    if (Video == null || !CanDelete)
                    {
                        return;
                    }

                    try
                    {
                        if (AuthenticationService.Current.IsLoggedIn)
                        {
                            if (await _vidMeClient.DeleteVideoAsync(Video.VideoId))
                            {
                                _videoLoadingViewModel.Items.Remove(this);
                            }
                        }
                        else
                        {
                            var key = Utils.GetAnonVideoKeyName(Video.VideoId);
                            var auth = _settingsService.Roaming.GetS<Auth>(key);

                            if (auth != null)
                            {
                                if (await _vidMeClient.DeleteAnonymousVideoAsync(Video.VideoId, auth.Token))
                                {
                                    _videoLoadingViewModel.Items.Remove(this);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                });
            }
        }

        public RelayCommand OpenVideoCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Messenger.Default.Send(new VideoMessage(this));
                    _navigationService.Navigate<VideoPlayerView>();
                });
            }
        }

        public RelayCommand RefreshCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    if (CommentCount > 0)
                    {
                        await LoadData(true);
                    }
                });
            }
        }

        public RelayCommand LoadMoreCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    if (CommentCount > 0 && CanLoadMore)
                    {
                        await LoadData(false, true, Comments.Count);
                    }
                });
            }
        }

        private bool VideoIsAnonymousButOwned()
        {
            // This means it's an anonymous video
            return string.IsNullOrEmpty(Video.UserId) && _settingsService.Roaming.Contains(Utils.GetAnonVideoKeyName(Video.VideoId));
        }

        public ListType ListType { get { return ListType.Normal; } }

        public async Task LoadData(bool isRefresh, bool add = false, int offset = 0)
        {
            if (_commentsLoaded && !isRefresh && !add)
            {
                return;
            }

            try
            {
                if (!add)
                {
                    SetProgressBar("Getting comments...");
                }

                IsLoadingMore = add;

                var response = await _vidMeClient.GetCommentsAsync(Video.VideoId, SortDirection.Ascending, offset);

                if (Comments == null || !add)
                {
                    Comments = new ObservableCollection<CommentViewModel>();
                }

                Comments.AddRange(response.Comments.Select(x => new CommentViewModel(x, this)));

                IsEmpty = Comments.IsNullOrEmpty();
                CanLoadMore = response.Page.Total > Comments.Count;
                _commentsLoaded = true;
            }
            catch (Exception ex)
            {
                
            }

            IsLoadingMore = false;
            SetProgressBar();
        }
    }
}
