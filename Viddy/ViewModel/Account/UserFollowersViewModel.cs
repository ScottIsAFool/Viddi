using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Cimbalino.Toolkit.Extensions;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Messaging;
using Viddy.Extensions;
using Viddy.Messaging;
using Viddy.ViewModel.Item;
using VidMePortable;

namespace Viddy.ViewModel.Account
{
    public class UserFollowersViewModel : LoadingItemsViewModel<UserViewModel>
    {
        private readonly INavigationService _navigationService;
        private readonly IVidMeClient _vidMeClient;

        private UserViewModel _user;

        public UserFollowersViewModel(INavigationService navigationService, IVidMeClient vidMeClient)
        {
            _navigationService = navigationService;
            _vidMeClient = vidMeClient;
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
                    SetProgressBar("Getting followers...");
                }

                IsLoadingMore = add;

                var response = await _vidMeClient.GetUsersFollowersAsync(_user.User.UserId, offset);
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

            IsEmpty = Items.IsNullOrEmpty();
            SetProgressBar();
        }

        protected override void WireMessages()
        {
            Messenger.Default.Register<UserMessage>(this, m =>
            {
                if (m.Notification.Equals(Constants.Messages.UserDetailMsg))
                {
                    if (_user == null || _user.User.UserId != m.User.User.UserId)
                    {
                        Reset();

                        _user = m.User;
                    }
                }
            });
        }
    }
}
