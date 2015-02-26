using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Cimbalino.Toolkit.Extensions;
using Viddy.ViewModel.Item;
using VidMePortable;

namespace Viddy.ViewModel
{
    public class BrowseChannelsViewModel : LoadingItemsViewModel<ChannelItemViewModel>
    {
        private readonly IVidMeClient _vidMeClient;

        public BrowseChannelsViewModel(IVidMeClient vidMeClient)
        {
            _vidMeClient = vidMeClient;
        }

        protected override async Task LoadData(bool isRefresh, bool add = false, int offset = 0)
        {
            if (ItemsLoaded && !add && !isRefresh)
            {
                return;
            }

            HasErrors = false;

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

                    CanLoadMore = false;
                    ItemsLoaded = true;
                }
            }
            catch (Exception ex)
            {
                HasErrors = true;
            }

            IsLoadingMore = false;
            SetProgressBar();
        }
    }
}
