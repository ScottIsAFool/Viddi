using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using JetBrains.Annotations;
using Viddy.Services;
using VidMePortable;
using VidMePortable.Model;

namespace Viddy.ViewModel.Item
{
    public class ChannelItemViewModel : ViewModelBase, IProfileViewModel, IFollowViewModel
    {
        private readonly IVidMeClient _vidMeClient;

        public ChannelItemViewModel(Channel channel)
        {
            Channel = channel;
            _vidMeClient = SimpleIoc.Default.GetInstance<IVidMeClient>();
        }

        public Channel Channel { get; set; }

        public string UserFollowers
        {
            get { return Channel != null && Channel.FollowerCount > 0 ? string.Format("{0:N0} followers", Channel.FollowerCount) : null; }
        }

        public string UserPlays
        {
            get
            {
                return null;
            }
        }
        public string Description { get { return Channel != null ? Channel.Description : null; } }
        public string CoverUrl { get { return Channel != null ? Channel.CoverUrl : null; } }
        public string AvatarUrl { get { return Channel != null ? Channel.AvatarUrl : null; } }

        public string Name
        {
            get { return Channel != null ? Channel.Title : null; }
        }

        public string UserVideoCount
        {
            get { return Channel != null && Channel.VideoCount > 0 ? string.Format("{0:N0} videos", Channel.VideoCount) : null; }
        }

        public bool DisplayBio
        {
            get { return Channel != null && !string.IsNullOrEmpty(Channel.Description); }
        }

        public bool DisplayByLine
        {
            get { return Channel != null && (!string.IsNullOrEmpty(UserFollowers) || !string.IsNullOrEmpty(UserPlays) || !string.IsNullOrEmpty(UserVideoCount)); }
        }

        public bool IsFollowedByMe { get; set; }
        public bool ChangingFollowState { get; set; }

        public string FollowingText
        {
            get
            {
                return IsFollowedByMe ? "following" : "follow";
            }
        }

        public bool CanFollow
        {
            get
            {
                return !ChangingFollowState
                       && AuthenticationService.Current.IsLoggedIn;
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
                    ? _vidMeClient.FollowChannelAsync(Channel.ChannelId)
                    : _vidMeClient.UnFollowChannelAsync(Channel.ChannelId);
                if (!await task)
                {
                    SetIsFollowedByMe(!isFollowing);
                }
                else
                {
                    if (isFollowing)
                    {
                        Channel.FollowerCount++;
                    }
                    else
                    {
                        Channel.FollowerCount--;
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
            try
            {
                var response = await _vidMeClient.IsUserFollowingChannelAsync(Channel.ChannelId);

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
    }
}
