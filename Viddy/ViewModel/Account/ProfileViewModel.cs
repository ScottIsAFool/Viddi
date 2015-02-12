using System.Collections.Generic;
using System.Threading.Tasks;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Viddy.Messaging;
using Viddy.Services;
using Viddy.ViewModel.Item;
using VidMePortable;
using VidMePortable.Model;

namespace Viddy.ViewModel.Account
{
    public class ProfileViewModel : ViewModelBase, IBackSupportedViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IVidMeClient _vidMeClient;
        private readonly ITileService _tileService;

        private Stack<UserViewModel> _previousItems; 

        public ProfileViewModel(INavigationService navigationService, IVidMeClient vidMeClient, ITileService tileService)
        {
            _navigationService = navigationService;
            _vidMeClient = vidMeClient;
            _tileService = tileService;

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
                }) {IsEmpty = true};
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

        public RelayCommand PageLoadedCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    if (User != null)
                    {
                        User.RefreshFollowerDetails().ConfigureAwait(false);
                        await User.PageLoaded();
                    }
                });
            }
        }

        public override string GetPinFileName(bool isWideTile = false)
        {
            return _tileService.GetTileFileName(TileService.TileType.User, User.User.UserId, isWideTile);
        }

        public override async Task PinUnpin()
        {
            if (IsPinned)
            {
                await _tileService.UnpinUser(User.User.UserId);
            }
            else
            {
                await _tileService.PinUser(User.User);
            }

            RaisePropertyChanged(() => IsPinned);
        }

        public override bool IsPinned
        {
            get { return User != null && User.IsPinned; }
        }

        protected override void WireMessages()
        {
            Messenger.Default.Register<UserMessage>(this, m =>
            {
                if (string.IsNullOrEmpty(m.Notification))
                {
                    
                    User = m.User;
                }
            });

            base.WireMessages();
        }

        public void ChangeContext()
        {
            if (_previousItems == null || _previousItems.Count == 0)
            {
                return;
            }

            var item = _previousItems.Pop();
            if (item != null)
            {
                User = item;
            }
        }

        public void SaveContext()
        {
            if (_previousItems == null)
            {
                _previousItems = new Stack<UserViewModel>();
            }

            _previousItems.Push(User);
        }
    }
}
