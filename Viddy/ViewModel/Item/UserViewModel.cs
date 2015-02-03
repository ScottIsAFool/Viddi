using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using JetBrains.Annotations;
using Viddy.Services;
using VidMePortable;
using VidMePortable.Model;

namespace Viddy.ViewModel.Item
{
    public class UserViewModel : ViewModelBase, IProfileViewModel, IFollowViewModel
    {
        private readonly IVidMeClient _vidMeClient;
        public User User { get; set; }

        public UserViewModel(User user)
        {
            User = user;
            _vidMeClient = SimpleIoc.Default.GetInstance<IVidMeClient>();
        }

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

        public string Description { get { return User != null ? User.Bio : null; } }
        public string CoverUrl { get { return User != null ? User.CoverUrl : null; } }
        public string AvatarUrl { get { return User != null ? User.AvatarUrl : null; } }
        public string Name { get { return User != null ? User.Username : null; } }

        public bool IsFollowedByMe { get; set; }
        public bool ChangingFollowState { get; set; }

        public string FollowingText
        {
            get
            {
                if (User != null
                    && User.UserId == AuthenticationService.Current.LoggedInUserId)
                {
                    return "this is you";
                }

                return IsFollowedByMe ? "following" : "follow";
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
            if (User.UserId == AuthenticationService.Current.LoggedInUserId)
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

        private void SetIsFollowedByMe(bool value)
        {
            _ignoreFollowedChanged = true;
            IsFollowedByMe = value;
            _ignoreFollowedChanged = false;
        }

        public string UserVideoCount
        {
            get { return User != null && User.VideoCount > 0 ? string.Format("{0:N0} videos", User.VideoCount) : null; }
        }

        public bool DisplayBio
        {
            get { return User != null && !string.IsNullOrEmpty(User.Bio); }
        }

        public bool DisplayByLine
        {
            get { return User != null && (!string.IsNullOrEmpty(UserFollowers) || !string.IsNullOrEmpty(UserPlays) || !string.IsNullOrEmpty(UserVideoCount)); }
        }
    }
}
