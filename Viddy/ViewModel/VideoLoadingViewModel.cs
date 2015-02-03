using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Cimbalino.Toolkit.Extensions;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Viddy.Extensions;
using Viddy.Model;
using Viddy.Services;
using Viddy.ViewModel.Item;
using VidMePortable.Model.Responses;

namespace Viddy.ViewModel
{
    public class VideoLoadingViewModel : ViewModelBase
    {
        private bool _videosLoaded;
        public bool CanLoadMore { get; set; }
        public bool IsLoadingMore { get; set; }
        public ObservableCollection<IListType> Items { get; set; }
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

        public virtual async Task<VideosResponse> GetVideos(int offset)
        {
            return new VideosResponse();
        }

        protected virtual void Reset()
        {
            Items = null;
            _videosLoaded = false;
            CanLoadMore = false;
            IsLoadingMore = false;
            IsEmpty = true;
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

                if (Items == null || !add)
                {
                    Items = new ObservableCollection<IListType>();
                }

                if (response != null && response.Videos != null)
                {
                    var allVideos = response.Videos.Select(x => new VideoItemViewModel(x, this)).ToList();

                    IEnumerable<IListType> videoList;

                    if (IncludeReviewsInFeed() && !allVideos.IsNullOrEmpty())
                    {
                        videoList = allVideos.AddEveryOften(10, 2, ReviewService.Current.ReviewViewModel);
                    }
                    else
                    {
                        videoList = allVideos;
                    }

                    //allVideos.AddEveryOften(10, 2, new AdViewModel(), 5, true);

                    Items.AddRange(videoList);
                }

                IsEmpty = Items.IsNullOrEmpty();
                CanLoadMore = response != null && response.Page != null && Items.Count + 1 < response.Page.Total;
                _videosLoaded = true;
            }
            catch (Exception ex)
            {

            }

            IsLoadingMore = false;
            SetProgressBar();
        }

        protected virtual bool IncludeReviewsInFeed()
        {
#if DEBUG
            return true;
#else
            return ReviewService.Current.CanShowReviews;
#endif
        }
        
        protected override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, m =>
            {
                if (m.Notification.Equals(Constants.Messages.HideReviewsMsg))
                {
                    if (Items == null)
                    {
                        return;
                    }

                    var reviews = Items.Where(x => x is ReviewViewModel).ToList();
                    if (reviews == null) return;
                    foreach (var item in reviews)
                    {
                        Items.Remove(item);
                    }
                }
            });
        }
    }
}
