using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Cimbalino.Toolkit.Extensions;
using Cimbalino.Toolkit.Services;
using Viddy.Extensions;
using Viddy.ViewModel.Item;
using VidMePortable;
using VidMePortable.Model;
using VidMePortable.Model.Responses;

namespace Viddy.ViewModel.Account
{
    public class UserPlaylistsViewModel : LoadingItemsViewModel<UserViewModel>
    {
        private readonly INavigationService _navigationService;
        private readonly IVidMeClient _vidMeClient;

        public UserPlaylistsViewModel(INavigationService navigationService, IVidMeClient vidMeClient)
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

                var response = new UsersResponse();
                if (response != null && !response.Users.IsNullOrEmpty())
                {
                    if (Items == null || !add)
                    {
                        Items = new ObservableCollection<UserViewModel>();
                    }

                    var allUsers = response.Users.Select(x => new UserViewModel(x)).ToList();

                    Items.AddRange(allUsers);
                }
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
