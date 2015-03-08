using System;
using System.Threading.Tasks;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using Viddi.Core;
using Viddi.Core.Services;
using Viddi.Localisation;
using Viddi.Messaging;
using Viddi.Services;
using Viddi.ViewModel.Account;
using Viddi.Views.Account;
using VidMePortable;
using VidMePortable.Model;
using VidMePortable.Model.Responses;

namespace Viddi.ViewModel.Item
{
    public class UserViewModel : VideoLoadingViewModel, IProfileViewModel, IFollowViewModel, ICanShowFollowers
    {
        private readonly IVidMeClient _vidMeClient;
        private readonly ITileService _tileService;
        private readonly INavigationService _navigationService;

        public User User { get; set; }

        public UserViewModel(User user)
        {
            User = user;
            _vidMeClient = SimpleIoc.Default.GetInstance<IVidMeClient>();
            _tileService = SimpleIoc.Default.GetInstance<ITileService>();
            _navigationService = SimpleIoc.Default.GetInstance<INavigationService>();
        }

        private FollowersViewModel _followers;
        public FollowersViewModel Followers
        {
            get { return _followers ?? (_followers = new FollowersViewModel(_vidMeClient, true)); }
        }

        public override Task<VideosResponse> GetVideos(int offset)
        {
            return _vidMeClient.GetUserVideosAsync(User.UserId, offset);
        }

        public bool IsShadowHeader { get; set; }
        public override bool IsPinned
        {
            get { return _tileService.IsUserPinned(User.UserId); }
        }

        #region IProfileViewModel implementations
        public string UserFollowers
        {
            get
            {
                if (User == null || IsShadowHeader)
                {
                    return string.Empty;
                }

                if (User.FollowerCount == 0)
                {
                    return Resources.ZeroFollowers;
                }


                return User.FollowerCount > 1 ? string.Format(Resources.UserFollowers, User.FollowerCount) : Resources.OneFollower;
            }
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
                    ? string.Format(Resources.UserPlays, views)
                    : null;
            }
        }
        
        public string Description { get { return User != null ? User.Bio : null; } }
        public string CoverUrl { get { return User != null ? User.CoverUrl : null; } }
        public string AvatarUrl { get { return User != null ? User.AvatarUrl : null; } }
        public string Name { get { return User != null ? User.Username : null; } }
        public string UserVideoCount
        {
            get { return User != null && User.VideoCount > 0 ? string.Format(Resources.UserVideoCount, User.VideoCount) : null; }
        }

        public bool DisplayBio
        {
            get { return User != null && !string.IsNullOrEmpty(User.Bio); }
        }

        public bool DisplayByLine
        {
            get { return User != null && (!string.IsNullOrEmpty(UserFollowers) || !string.IsNullOrEmpty(UserPlays) || !string.IsNullOrEmpty(UserVideoCount)); }
        }

        public bool IsNsfw { get { return false; } }

        #endregion

        #region IFollowViewModel implementations
        public bool IsFollowedByMe { get; set; }
        public bool ChangingFollowState { get; set; }

        public string FollowingText
        {
            get
            {
                if (User != null
                    && User.UserId == AuthenticationService.Current.LoggedInUserId)
                {
                    return Resources.ThisIsYou;
                }

                return IsFollowedByMe ? Resources.Following : Resources.Follow;
            }
        }

        public bool CanFollow
        {
            get
            {
                return !ChangingFollowState
                       && AuthenticationService.Current.IsLoggedIn
                       && User != null
                       && User.UserId != AuthenticationService.Current.LoggedInUserId;
            }
        }

        public async Task ChangeFollowingState(bool isFollowing)
        {
            try
            {
                ChangingFollowState = true;
                var task = isFollowing
                    ? _vidMeClient.FollowUserAsync(User.UserId)
                    : _vidMeClient.UnfollowUserAsync(User.UserId);
                if (!await task)
                {
                    SetIsFollowedByMe(!isFollowing);
                }
                else
                {
                    if (isFollowing)
                    {
                        User.FollowerCount++;
                    }
                    else
                    {
                        User.FollowerCount--;
                    }

                    RaisePropertyChanged(() => UserFollowers);
                }
            }
            catch (Exception ex)
            {

            }

            ChangingFollowState = false;
        }

        public async Task RefreshFollowerDetails()
        {
            if (!AuthenticationService.Current.IsLoggedIn || User.UserId == AuthenticationService.Current.LoggedInUserId)
            {
                return;
            }

            try
            {
                var response = await _vidMeClient.IsUserFollowingUserAsync(User.UserId, AuthenticationService.Current.LoggedInUserId);

                SetIsFollowedByMe(response);
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        private bool _ignoreFollowedChanged;

        [UsedImplicitly]
        private async void OnIsFollowedByMeChanged()
        {
            if (_ignoreFollowedChanged)
            {
                return;
            }

            await ChangeFollowingState(IsFollowedByMe);
        }

        private void SetIsFollowedByMe(bool value)
        {
            _ignoreFollowedChanged = true;
            IsFollowedByMe = value;
            _ignoreFollowedChanged = false;
        }

        public RelayCommand NavigateToProfileCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Messenger.Default.Send(new UserMessage(this));
                    _navigationService.Navigate<ProfileView>();
                });
            }
        }

        public RelayCommand NavigateToFollowersCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Messenger.Default.Send(new UserMessage(this, Constants.Messages.UserDetailMsg));
                    _navigationService.Navigate<UserFollowersView>();
                });
            }
        }

        #region ICanShowFollowers implementations
        public string Id { get { return User != null ? User.UserId : string.Empty; } }
        #endregion
    }
}
