using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;

namespace Viddy.ViewModel
{
    public abstract class LoadingItemsViewModel<TItemType> : ViewModelBase
    {
        protected bool ItemsLoaded;
        public bool CanLoadMore { get; set; }
        public bool IsLoadingMore { get; set; }
        public ObservableCollection<TItemType> Items { get; set; }
        public bool IsEmpty { get; set; }

        public RelayCommand PageLoadedCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await PageLoaded();
                });
            }
        }

        public RelayCommand RefreshCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await LoadData(true);
                });
            }
        }

        public RelayCommand LoadMoreCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await LoadData(false, true, Items.Count);
                });
            }
        }

        public virtual async Task PageLoaded()
        {
            await LoadData(false);
        }

        protected virtual async Task LoadData(bool isRefresh, bool add = false, int offset = 0)
        {
        }
        
        protected virtual void Reset()
        {
            Items = null;
            ItemsLoaded = false;
            CanLoadMore = false;
            IsLoadingMore = false;
            IsEmpty = true;
        }
    }
}