using System;
using System.Threading.Tasks;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using Viddy.Core;
using Viddy.Core.Services;
using Viddy.Localisation;
using Viddy.Messaging;
using Viddy.Services;
using Viddy.ViewModel.Account;
using Viddy.Views;
using Viddy.Views.Account;
using VidMePortable;
using VidMePortable.Model;
using VidMePortable.Model.Responses;

namespace Viddy.ViewModel.Item
{
    public class ChannelItemViewModel : VideoLoadingViewModel, IProfileViewModel, IFollowViewModel, ICanShowFollowers
    {
        private readonly IVidMeClient _vidMeClient;
        private readonly INavigationService _navigationService;
        private readonly ITileService _tileService;

        public ChannelItemViewModel(Channel channel)
        {
            Channel = channel;
            _vidMeClient = SimpleIoc.Default.GetInstance<IVidMeClient>();
            _navigationService = SimpleIoc.Default.GetInstance<INavigationService>();
            _tileService = SimpleIoc.Default.GetInstance<ITileService>();
        }

        public Channel Channel { get; set; }

        public override bool IsPinned
        {
            get { return _tileService.IsChannelPinned(Channel.ChannelId); }
        }

        #region IProfileViewModel implementations
        public string UserFollowers
        {
            get
            {
                if (Channel == null)
                {
                    return string.Empty;
                }

                if (Channel.FollowerCount == 0)
                {
                    return Resources.ZeroFollowers;
                }


                return Channel.FollowerCount > 1 ? string.Format(Resources.UserFollowers, Channel.FollowerCount) : Resources.OneFollower;
            }
        }

        public string UserPlays { get { return null; } }
        public string Description { get { return Channel != null ? Channel.Description : null; } }
        public string CoverUrl { get { return Channel != null ? Channel.CoverUrl : null; } }
        public string AvatarUrl { get { return Channel != null ? Channel.AvatarUrl : null; } }

        public string Name
        {
            get { return Channel != null ? Channel.Title : null; }
        }

        public string UserVideoCount
        {
            get { return Channel != null && Channel.VideoCount > 0 ? string.Format(Resources.UserVideoCount, Channel.VideoCount) : null; }
        }

        public bool DisplayBio
        {
            get { return Channel != null && !string.IsNullOrEmpty(Channel.Description); }
        }

        public bool DisplayByLine
        {
            get { return Channel != null && (!string.IsNullOrEmpty(UserFollowers) || !string.IsNullOrEmpty(UserPlays) || !string.IsNullOrEmpty(UserVideoCount)); }
        }

        public bool IsNsfw { get { return Channel != null && Channel.Nsfw; } }

        #endregion

        #region IFollowViewModel implementation
        public bool IsFollowedByMe { get; set; }
        public bool ChangingFollowState { get; set; }

        public string FollowingText
        {
            get
            {
                return IsFollowedByMe ? Resources.Following : Resources.Follow;
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
            if (!AuthenticationService.Current.IsLoggedIn)
            {
                return;
            }

            try
            {
                var response = await _vidMeClient.IsUserFollowingChannelAsync(Channel.ChannelId);

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

        public override Task<VideosResponse> GetVideos(int offset)
        {
            return _vidMeClient.GetChannelsNewVideosAsync(Channel.ChannelId, offset);
        }

        public RelayCommand NavigateToChannel
        {
            get
            {
                return new RelayCommand(() =>
                    {
                        Messenger.Default.Send(new ChannelMessage(this));
                        _navigationService.Navigate<ChannelView>();
                    });
            }
        }

        public RelayCommand NavigateToFollowersCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Messenger.Default.Send(new ChannelMessage(this, Constants.Messages.UserDetailMsg));
                    _navigationService.Navigate<UserFollowersView>();
                });
            }
        }

        #region ICanShowFollowers implementations
        public string Id { get { return Channel != null ? Channel.ChannelId : string.Empty; } }
        private FollowersViewModel _followers;

        public FollowersViewModel Followers
        {
            get { return _followers ?? (_followers = new FollowersViewModel(_vidMeClient, false)); }
        }
        #endregion
    }
}
