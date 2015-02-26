using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Viddy.Core.Extensions;
using Viddy.Core.Model;
using Viddy.Messaging;
using Viddy.Services;
using Viddy.ViewModel.Item;
using Viddy.Views;
using Viddy.Views.Account;
using VidMePortable;
using VidMePortable.Model;

namespace Viddy.ViewModel.Account
{
    public class ProfileViewModel : ViewModelBase, IBackSupportedViewModel, ICanHasHomeButton
    {
        private readonly INavigationService _navigationService;
        private readonly IVidMeClient _vidMeClient;
        private readonly ITileService _tileService;

        private Stack<UserViewModel> _previousItems;
        private bool _fromProtocol;

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
                });
            }
        }

        public UserViewModel User { get; set; }

        public UserViewModel EmptyUser
        {
            get { return new UserViewModel(new User()) {IsShadowHeader = true}; }
        }

        public RelayCommand PageLoadedCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    if (!_fromProtocol)
                    {
                        await LoadUserVideos();
                    }
                });
            }
        }

        private async Task GetUser(string userId)
        {
            try
            {
                var response = await _vidMeClient.GetUserAsync(userId);
                if (response != null)
                {
                    if (User == null)
                    {
                        User = new UserViewModel(response.User);
                    }
                    else
                    {
                        User.User = response.User;
                    }

                    await LoadUserVideos();
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        private async Task LoadUserVideos()
        {
            if (User != null)
            {
                User.RefreshFollowerDetails().ConfigureAwait(false);
                await User.PageLoaded();
            }
        }

        public override string GetPinFileName(bool isWideTile = false)
        {
            return _tileService.GetTileFileName(TileType.User, User.User.UserId, isWideTile);
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
                    _fromProtocol = false;
                    User = m.User;
                }
            });

            Messenger.Default.Register<ProtocolMessage>(this, m =>
            {
                if (m.Type != ProtocolMessage.ProtocolType.User)
                {
                    return;
                }

                _fromProtocol = true;
                User = new UserViewModel(new User()) {ProgressIsVisible = true};
                GetUser(m.Content);
            });

            base.WireMessages();
        }

        public void ChangeContext(Type callingType)
        {
            if (_previousItems.IsNullOrEmpty() || callingType != typeof(ProfileView))
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

        public bool ShowHomeButton { get; set; }

        public RelayCommand NavigateHomeCommand
        {
            get
            {
                return new RelayCommand(() => _navigationService.Navigate<MainView>());
            }
        }
    }
}
