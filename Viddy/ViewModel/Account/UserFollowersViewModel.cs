using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Cimbalino.Toolkit.Extensions;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Messaging;
using Viddy.Extensions;
using Viddy.Messaging;
using Viddy.ViewModel.Item;
using Viddy.Views.Account;
using VidMePortable;
using VidMePortable.Model.Responses;

namespace Viddy.ViewModel.Account
{
    public class FollowersViewModel : LoadingItemsViewModel<UserViewModel>
    {
        private readonly IVidMeClient _vidMeClient;
        private readonly bool _isUser;

        public FollowersViewModel(IVidMeClient vidMeClient, bool isUser)
        {
            _vidMeClient = vidMeClient;
            _isUser = isUser;
        }

        public string Id { get; set; }

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
                    SetProgressBar("Getting followers...");
                }

                IsLoadingMore = add;

                var response = await GetTask(Id, offset);
                if (response != null && !response.Users.IsNullOrEmpty())
                {
                    if (Items == null || !add)
                    {
                        Items = new ObservableCollection<UserViewModel>();
                    }

                    var allUsers = response.Users.Select(x => new UserViewModel(x)).ToList();

                    Items.AddRange(allUsers);
                    CanLoadMore = Items != null && Items.Count < response.Page.Total;
                }

                ItemsLoaded = true;
            }
            catch (Exception ex)
            {

            }

            IsLoadingMore = false;
            IsEmpty = Items.IsNullOrEmpty();
            SetProgressBar();
        }

        private Task<UsersResponse> GetTask(string id, int offset)
        {
            return _isUser
                ? _vidMeClient.GetUsersFollowersAsync(id, offset)
                : _vidMeClient.GetChannelFollowersAsync(id, offset);
        }
    }

    public class UserFollowersViewModel : ViewModelBase, IBackSupportedViewModel
    {
        private readonly INavigationService _navigationService;

        public UserFollowersViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        private Stack<ICanShowFollowers> _previousItems;

        public ICanShowFollowers Item { get; set; }

        protected override void WireMessages()
        {
            Messenger.Default.Register<UserMessage>(this, m =>
            {
                if (m.Notification.Equals(Constants.Messages.UserDetailMsg))
                {
                    var user = Item as UserViewModel;
                    if (user == null || user.Id != m.User.Id)
                    {
                        Item = m.User;
                        Item.Followers.Id = Item.Id;
                    }
                }
            });

            Messenger.Default.Register<ChannelMessage>(this, m =>
            {
                if (m.Notification.Equals(Constants.Messages.UserDetailMsg))
                {
                    var channel = Item as ChannelItemViewModel;
                    if (channel == null || channel.Id != m.Channel.Id)
                    {
                        Item = m.Channel;
                        Item.Followers.Id = Item.Id;
                    }
                }
            });
        }

        #region IBackSupportedViewModel implementations
        public void ChangeContext(Type callingType)
        {
            if (_previousItems.IsNullOrEmpty() || callingType != typeof(UserFollowersView))
            {
                return;
            }

            var item = _previousItems.Pop();
            if (item != null)
            {
                Item = item;
            }
        }

        public void SaveContext()
        {
            if (_previousItems == null)
            {
                _previousItems = new Stack<ICanShowFollowers>();
            }

            _previousItems.Push(Item);
        }
        #endregion
    }
}
