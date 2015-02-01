using System;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Viddy.Messaging;
using Viddy.Model;
using Viddy.Services;
using Viddy.Views.Account;
using VidMePortable;
using VidMePortable.Model;

namespace Viddy.ViewModel.Item
{
    public class CommentViewModel : ViewModelBase, IListType
    {
        private readonly VideoItemViewModel _videoItemViewModel;
        private readonly IVidMeClient _vidMeClient;
        private readonly IMessageBoxService _messageBoxService;
        private readonly INavigationService _navigationService;

        public CommentViewModel(Comment comment, VideoItemViewModel videoItemViewModel)
        {
            _videoItemViewModel = videoItemViewModel;
            Comment = comment;
            _vidMeClient = SimpleIoc.Default.GetInstance<IVidMeClient>();
            _messageBoxService = SimpleIoc.Default.GetInstance<IMessageBoxService>();
            _navigationService = SimpleIoc.Default.GetInstance<INavigationService>();

            if (IsInDesignMode)
            {
                Comment = new Comment
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
                };
            }
        }

        public ListType ListType { get { return ListType.Normal; } }

        public Comment Comment { get; set; }

        public string UserAvatar
        {
            get
            {
                return Comment != null && !string.IsNullOrEmpty(Comment.UserId)
                    ? _vidMeClient.GetUserAvatar(Comment.UserId)
                    : string.Empty;
            }
        }

        public bool CanDelete
        {
            get
            {
                return Comment != null
                    && AuthenticationService.Current.IsLoggedIn
                    && AuthenticationService.Current.LoggedInUserId == Comment.UserId;
            }
        }

        public string Date
        {
            get { return Comment != null && Comment.DateCreated.HasValue ? Utils.DaysAgo(Comment.DateCreated.Value) : string.Empty; }
        }

        public string Username
        {
            get { return Comment != null && Comment.User != null ? Comment.User.Username : "Unknown user"; }
        }

        public RelayCommand DeleteCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    try
                    {
                        if (!CanDelete)
                        {
                            return;
                        }

                        if (await _vidMeClient.DeleteCommentAsync(Comment.CommentId))
                        {
                            _videoItemViewModel.RemoveComment(this);
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                });
            }
        }

        public RelayCommand NavigateTouserCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Messenger.Default.Send(new UserMessage(Comment.User));
                    _navigationService.Navigate<ProfileView>();
                });
            }
        }
    }
}
