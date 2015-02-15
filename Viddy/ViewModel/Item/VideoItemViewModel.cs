using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
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

namespace Viddy.ViewModel.Item
{
    public class VideoItemViewModel : LoadingItemsViewModel<CommentViewModel>, IListType
    {
        private readonly IVidMeClient _vidMeClient;
        private readonly VideoLoadingViewModel _videoLoadingViewModel;
        private readonly IApplicationSettingsService _settingsService;
        private readonly INavigationService _navigationService;
        private readonly ITileService _tileService;

        private readonly DataTransferManager _manager;
        
        public VideoItemViewModel(Video video, VideoLoadingViewModel videoLoadingViewModel)
        {
            _vidMeClient = SimpleIoc.Default.GetInstance<IVidMeClient>();
            _settingsService = SimpleIoc.Default.GetInstance<IApplicationSettingsService>();
            _navigationService = SimpleIoc.Default.GetInstance<INavigationService>();
            _tileService = SimpleIoc.Default.GetInstance<ITileService>();
            _manager = DataTransferManager.GetForCurrentView();
            _videoLoadingViewModel = videoLoadingViewModel;
            Video = video;

            if (Video != null && Video.Channel != null)
            {
                Channel = new ChannelItemViewModel(Video.Channel);
            }

            if (IsInDesignMode)
            {
                var user = new User
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
                Items = new ObservableCollection<CommentViewModel>
                {
                    new CommentViewModel(new Comment
                    {
                        Body = "This is a comment",
                        User = user
                    }, this)
                };

                Video = new Video
                {
                    User = user
                };
            }
        }

        public Video Video { get; set; }

        public ChannelItemViewModel Channel { get; set; }

        public override bool IsPinned
        {
            get { return _tileService.IsVideoPinned(Video.VideoId); }
        }

        public string Title
        {
            get { return Video != null && !string.IsNullOrEmpty(Video.Title) ? Video.Title : "Untitled"; }
        }

        public bool IsOwner
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

        public string ChannelInfo
        {
            get { return Video != null && !string.IsNullOrEmpty(Video.ChannelId) ? Video.ChannelId : null; }
        }

        public string Plays
        {
            get { return Video != null ? string.Format("{0} plays", Video.ViewCount) : "0 plays"; }
        }

        public string Date
        {
            get { return Video != null && Video.DateCreated.HasValue ? Utils.DaysAgo(Video.DateCreated.Value) : string.Empty; }
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

        public int UsersVote { get; set; }

        public void RemoveComment(CommentViewModel comment)
        {
            if (Items == null)
            {
                return;
            }

            Items.Remove(comment);
            Video.CommentCount--;
            RaisePropertyChanged(()=> CommentCount);
        }

        public RelayCommand DeleteCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    try
                    {
                        if (AuthenticationService.Current.IsLoggedIn)
                        {
                            if (await _vidMeClient.DeleteVideoAsync(Video.VideoId))
                            {
                                if (_videoLoadingViewModel != null)
                                {
                                    _videoLoadingViewModel.Items.Remove(this);
                                }
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
                                    if (_videoLoadingViewModel != null)
                                    {
                                        _videoLoadingViewModel.Items.Remove(this);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }, () => IsOwner);
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

        public RelayCommand AddCommentCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    try
                    {
                        AddingComment = true;
                        var now = DateTime.Now;
                        var response = await _vidMeClient.CreateCommentAsync(Video.VideoId, CommentText, now.TimeOfDay);
                        if (response != null)
                        {
                            response.User = AuthenticationService.Current.LoggedInUser;

                            var commentVm = new CommentViewModel(response, this);
                            if (Items == null)
                            {
                                Items = new ObservableCollection<CommentViewModel>();
                            }

                            Items.Insert(0, commentVm);

                            CommentText = string.Empty;
                            Video.CommentCount++;
                        }
                    }
                    catch (Exception ex)
                    {

                    }

                    AddingComment = false;
                }, () => CanAddComment);
            }
        }

        public RelayCommand ShareCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    _manager.DataRequested += ManagerOnDataRequested;
                    DataTransferManager.ShowShareUI();
                });
            }
        }

        public RelayCommand RefreshVideoInfoCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await RefreshVideoDetails();
                });
            }
        }

        public RelayCommand UpVoteCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    var vote = UsersVote == 0 ? Vote.Up : Vote.Neutral;

                    await ChangeVote(vote);
                });
            }
        }

        public RelayCommand DownVoteCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    var vote = UsersVote == 0 ? Vote.Down : Vote.Neutral;

                    await ChangeVote(vote);
                });
            }
        }

        public RelayCommand EditVideoCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Messenger.Default.Send(new VideoMessage(this, Constants.Messages.EditVideoMsg));
                    _navigationService.Navigate<UploadVideoView>();
                }, () => IsOwner);
            }
        }

        private async Task ChangeVote(Vote vote)
        {
            GettingVideoInfo = true;
            ViewerVote response = null;
            try
            {
                response = await _vidMeClient.VoteForVideoAsync(Video.VideoId, vote);
            }
            catch (Exception ex)
            {
                GettingVideoInfo = false;
                return;
            }

            if (response != null)
            {
                await RefreshVideoDetails();                
            }
        }

        public string CommentText { get; set; }
        public bool AddingComment { get; set; }

        public string Background
        {
            get
            {
                if (Video != null && !string.IsNullOrEmpty(Video.Colors))
                {
                    var colours = Video.Colors.Split(',');
                    if (!colours.IsNullOrEmpty())
                    {
                        return colours.First();
                    }
                }

                return "#FFFFFFF";
            }
        }

        public bool CanAddComment
        {
            get
            {
                return !AddingComment
                       && !string.IsNullOrEmpty(CommentText)
                       && AuthenticationService.Current.IsLoggedIn;
            }
        }

        public ListType ListType { get { return ListType.Normal; } }

        public async Task RefreshVideoDetails()
        {
            try
            {
                GettingVideoInfo = true;
                var response = await _vidMeClient.GetVideoAsync(Video.VideoId);
                if (response != null)
                {
                    Video = response.Video;
                    UsersVote = response.ViewerVote != null && response.ViewerVote.Value.HasValue ? response.ViewerVote.Value.Value : 0;
                }
            }
            catch (Exception ex)
            {
                
            }

            GettingVideoInfo = false;
        }

        public bool GettingVideoInfo { get; set; }

        public async Task LoadData()
        {
            await LoadData(false);
        }

        protected override async Task LoadData(bool isRefresh, bool add = false, int offset = 0)
        {
            if (ItemsLoaded && !isRefresh && !add)
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

                if (Items == null || !add)
                {
                    Items = new ObservableCollection<CommentViewModel>();
                }

                Items.AddRange(response.Comments.Select(x => new CommentViewModel(x, this)));

                IsEmpty = Items.IsNullOrEmpty();
                CanLoadMore = response.Page.Total > Items.Count;
                ItemsLoaded = true;
            }
            catch (Exception ex)
            {

            }

            IsLoadingMore = false;
            SetProgressBar();
        }

        private void ManagerOnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            _manager.DataRequested -= ManagerOnDataRequested;
            var request = args.Request;
            request.Data.Properties.Title = string.IsNullOrEmpty(Title) ? Title : "Check out this video";
            var message = IsAnonymous ? string.Format("Check out this video") : string.Format("Check out this video by {0}", Video.User.Username);
            request.Data.Properties.Description = message;
            request.Data.SetUri(new Uri(Video.FullUrl));
        }

        private bool VideoIsAnonymousButOwned()
        {
            // This means it's an anonymous video
            return string.IsNullOrEmpty(Video.UserId) && _settingsService.Roaming.Contains(Utils.GetAnonVideoKeyName(Video.VideoId));
        }
    }
}
