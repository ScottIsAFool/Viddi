using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Cimbalino.Toolkit.Extensions;
using GalaSoft.MvvmLight.Messaging;
using Viddy.Extensions;
using Viddy.Model;
using Viddy.Services;
using Viddy.ViewModel.Item;
using VidMePortable.Model.Responses;

namespace Viddy.ViewModel
{
    public class VideoLoadingViewModel : LoadingItemsViewModel<IListType>
    {
        public virtual async Task<VideosResponse> GetVideos(int offset)
        {
            return new VideosResponse();
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
                ItemsLoaded = true;
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
