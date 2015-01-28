using System;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Viddy.Model;
using Viddy.Services;
using VidMePortable;
using VidMePortable.Model;

namespace Viddy.ViewModel
{
    public class CommentViewModel : ViewModelBase, IListType
    {
        private readonly VideoItemViewModel _videoItemViewModel;
        private readonly IVidMeClient _vidMeClient;
        private readonly IMessageBoxService _messageBoxService;

        public CommentViewModel(Comment comment, VideoItemViewModel videoItemViewModel)
        {
            _videoItemViewModel = videoItemViewModel;
            Comment = comment;
            _vidMeClient = SimpleIoc.Default.GetInstance<IVidMeClient>();
            _messageBoxService = SimpleIoc.Default.GetInstance<IMessageBoxService>();
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
                            _videoItemViewModel.Comments.Remove(this);
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                });
            }
        }
    }
}
