using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Viddy.Extensions;
using VidMePortable.Model.Responses;

namespace Viddy.ViewModel
{
    public class VideoLoadingViewModel : ViewModelBase
    {
        private bool _videosLoaded;
        public bool CanLoadMore { get; set; }
        public bool IsLoadingMore { get; set; }
        public ObservableCollection<VideoItemViewModel> Videos { get; set; }
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
                    await LoadData(false, true, Videos.Count);
                });
            }
        }

        public virtual async Task PageLoaded()
        {
            await LoadData(false);
        }

        public virtual async Task<VideosResponse> GetVideos(int offset)
        {
            return new VideosResponse();
        }

        protected void Reset()
        {
            Videos = null;
            _videosLoaded = false;
            CanLoadMore = false;
            IsLoadingMore = false;
            IsEmpty = false;
        }

        protected async Task LoadData(bool isRefresh, bool add = false, int offset = 0)
        {
            if (_videosLoaded && !isRefresh && !add)
            {
                return;
            }

            try
            {
                if (!add)
                {
                    SetProgressBar("Getting videos...");
                }

                IsLoadingMore = add;
                var response = await GetVideos(offset);

                if (Videos == null || !add)
                {
                    Videos = new ObservableCollection<VideoItemViewModel>();
                }

                foreach (var video in response.Videos.Select(x => new VideoItemViewModel(x)))
                {
                    Videos.Add(video);
                }

                IsEmpty = Videos.IsNullOrEmpty();
                CanLoadMore = Videos.Count + 1 < response.Page.Total;
                _videosLoaded = true;
            }
            catch (Exception ex)
            {

            }

            IsLoadingMore = false;
            SetProgressBar();
        }
    }
}
