using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Cimbalino.Toolkit.Extensions;
using Cimbalino.Toolkit.Services;
using Viddy.Extensions;
using Viddy.ViewModel.Item;
using VidMePortable;

namespace Viddy.ViewModel
{
    public class BrowseChannelsViewModel : LoadingItemsViewModel<ChannelItemViewModel>
    {
        private readonly INavigationService _navigationService;
        private readonly IVidMeClient _vidMeClient;

        public BrowseChannelsViewModel(INavigationService navigationService, IVidMeClient vidMeClient)
        {
            _navigationService = navigationService;
            _vidMeClient = vidMeClient;
        }

        protected override async Task LoadData(bool isRefresh, bool add = false, int offset = 0)
        {
            if (ItemsLoaded && !add && !isRefresh)
            {
                return;
            }

            try
            {
                if (!add)
                {
                    SetProgressBar("Getting channels...");
                }

                IsLoadingMore = add;

                var response = await _vidMeClient.GetChannelsAsync();
                if (response != null)
                {
                    if (Items == null || !add)
                    {
                        Items = new ObservableCollection<ChannelItemViewModel>();
                    }

                    Items.AddRange(response.Select(x => new ChannelItemViewModel(x)));

                    IsEmpty = Items.IsNullOrEmpty();
                    CanLoadMore = false;
                    ItemsLoaded = true;
                }
            }
            catch (Exception ex)
            {
                
            }

            IsLoadingMore = false;
            SetProgressBar();
        }
    }
}
