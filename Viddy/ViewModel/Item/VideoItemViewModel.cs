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
using Viddi.Core;
using Viddi.Core.Extensions;
using Viddi.Core.Services;
using Viddi.Localisation;
using Viddi.Messaging;
using Viddi.Model;
using Viddi.Services;
using Viddi.Views;
using VidMePortable;
using VidMePortable.Model;

namespace Viddi.ViewModel.Item
{
    public class VideoItemViewModel : LoadingItemsViewModel<CommentViewModel>, IListType
    {
        private readonly IVidMeClient _vidMeClient;
        private readonly VideoLoadingViewModel _videoLoadingViewModel;
        private readonly IApplicationSettingsService _settingsService;
        private readonly INavigationService _navigationService;
        private readonly ITileService _tileService;
        private readonly IToastService _toastService;

        private DataTransferManager _manager;
        private ShareType _shareType;

        public VideoItemViewModel(Video video, VideoLoadingViewModel videoLoadingViewModel)
        {
            _vidMeClient = SimpleIoc.Default.GetInstance<IVidMeClient>();
            _settingsService = SimpleIoc.Default.GetInstance<IApplicationSettingsService>();
            _navigationService = SimpleIoc.Default.GetInstance<INavigationService>();
            _tileService = SimpleIoc.Default.GetInstance<ITileService>();
            _toastService = SimpleIoc.Default.GetInstance<IToastService>();
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

                return IsAnonymous ? Resources.Anonymous : Video.User.Username;
            }
        }

        public string ChannelInfo
        {
            get { return Video != null && !string.IsNullOrEmpty(Video.ChannelId) ? Video.ChannelId : null; }
        }

        public string Plays
        {
            get
            {
                var number = Video != null ? Video.ViewCount : 0;
                return number == 1 ? Resources.OneUserPlay : string.Format(Resources.UserPlays, number);
            }
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

        public bool IsUpVote { get; set; }
        public bool IsDownVote { get; set; }

        public string Score
        {
            get
            {
                if (Video == null)
                {
                    return string.Empty;
                }

                var score = Video.Score;
                return score == 1 ? Resources.OnePoint : string.Format(Resources.Points, score);
            }
        }

        public void RemoveComment(CommentViewModel comment)
        {
            if (Items == null)
            {
                return;
            }

            Items.Remove(comment);
            Video.CommentCount--;
            RaisePropertyChanged(() => CommentCount);
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
                        Log.ErrorException("DeleteCommend(" + AuthenticationService.Current.IsLoggedIn + ")", ex);
                        _toastService.Show(Resources.ErrorDeletingComment);
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
                        Log.ErrorException("AddComment", ex);
                        _toastService.Show(Resources.ErrorAddingComment);
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
                    _shareType = ShareType.Info;
                    ShowShareUI();
                });
            }
        }

        public RelayCommand ShareLinkCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    _shareType = ShareType.JustLink;
                    ShowShareUI();
                });
            }
        }

        private void ShowShareUI()
        {
            _manager = DataTransferManager.GetForCurrentView();
            _manager.DataRequested += ManagerOnDataRequested;
            DataTransferManager.ShowShareUI();
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
                    var vote = !IsUpVote ? Vote.Up : Vote.Neutral;

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
                    var vote = !IsDownVote ? Vote.Down : Vote.Neutral;

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

        public RelayCommand OpenInBrowserCommand
        {
            get
            {
                return new RelayCommand(() => new LauncherService().LaunchUriAsync(Video.FullUrl));
            }
        }

        private async Task ChangeVote(Vote vote)
        {
            GettingVideoInfo = true;
            try
            {
                var response = await _vidMeClient.VoteForVideoAsync(Video.VideoId, vote);
                if (response != null)
                {
                    SetViewerVote(response.ViewerVote);
                    if (response.Video != null)
                    {
                        Video.LikesCount = response.Video.LikesCount;
                        Video.Score = response.Video.Score;
                        RaisePropertyChanged(() => Score);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.ErrorException("ChangeVote()", ex);
                return;
            }

            GettingVideoInfo = false;
        }

        private void SetViewerVote(ViewerVote viewerVote)
        {
            var value = viewerVote != null && viewerVote.Value.HasValue ? viewerVote.Value.Value : 0;
            IsUpVote = value > 0;
            IsDownVote = value < 0;
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
                    SetViewerVote(response.ViewerVote);
                }
            }
            catch (Exception ex)
            {
                Log.ErrorException("RefreshVideoDetails()", ex);
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

                CanLoadMore = response.Page.Total > Items.Count;
                ItemsLoaded = true;
            }
            catch (Exception ex)
            {
                HasErrors = true;
                Log.ErrorException("LoadData(Comments)", ex);
            }

            IsLoadingMore = false;
            SetProgressBar();
        }

        private void ManagerOnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            _manager.DataRequested -= ManagerOnDataRequested;
            var request = args.Request;
            request.Data.Properties.Title = Resources.ShareVideoTitle;
            var description = IsAnonymous ? Resources.ShareVideoTitle : string.Format(Resources.ShareVideoMessage, Video.User.Username);
            request.Data.Properties.Description = description;
            request.Data.SetApplicationLink(new Uri(Video.ToViddiLink()));

            switch (_shareType)
            {
                case ShareType.Info:
                    var message = PrepareMessage(description);
                    request.Data.SetText(message);
                    break;
                case ShareType.JustLink:
                    request.Data.SetWebLink(new Uri(Video.FullUrl));
                    break;
            }
        }

        private string PrepareMessage(string description)
        {
            description += "\n\n" + Video.FullUrl + "\n\n";
            var ViddiLink = string.Format(Resources.ViddiWindowsPhone, Video.ToViddiLink());

            description += ViddiLink;

            return description;
        }

        private bool VideoIsAnonymousButOwned()
        {
            // This means it's an anonymous video
            return string.IsNullOrEmpty(Video.UserId) && _settingsService.Roaming.Contains(Utils.GetAnonVideoKeyName(Video.VideoId));
        }

        private enum ShareType
        {
            JustLink,
            Info
        }
    }
}
