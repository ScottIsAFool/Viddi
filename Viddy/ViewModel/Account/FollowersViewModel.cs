using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Cimbalino.Toolkit.Extensions;
using Viddy.Core.Extensions;
using Viddy.Extensions;
using Viddy.ViewModel.Item;
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
}