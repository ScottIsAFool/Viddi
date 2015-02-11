using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Cimbalino.Toolkit.Extensions;
using Cimbalino.Toolkit.Services;
using Viddy.Extensions;
using Viddy.Services;
using Viddy.ViewModel.Item;
using VidMePortable;

namespace Viddy.ViewModel.Account
{
    public class NotificationsViewModel : LoadingItemsViewModel<NotificationItemViewModel>
    {
        private readonly INavigationService _navigationService;
        private readonly IVidMeClient _vidMeClient;
        private readonly INotificationService _notificationService;

        public NotificationsViewModel(INavigationService navigationService, IVidMeClient vidMeClient, INotificationService notificationService)
        {
            _navigationService = navigationService;
            _vidMeClient = vidMeClient;
            _notificationService = notificationService;
        }

        public override Task PageLoaded()
        {
            _notificationService.MarkAllAsRead();
            return base.PageLoaded();
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
                    SetProgressBar("Getting notifications");
                }

                IsLoadingMore = add;

                var response = await _vidMeClient.GetNotificationsAsync(offset: offset);
                if (response != null && !response.Notifications.IsNullOrEmpty())
                {
                    if (Items == null || !add)
                    {
                        Items = new ObservableCollection<NotificationItemViewModel>();
                    }

                    var allNotifications = response.Notifications.Select(x => new NotificationItemViewModel(x)).ToList();

                    Items.AddRange(allNotifications);
                    //CanLoadMore = Items != null && Items.Count < response.page

                    ItemsLoaded = true;
                }
            }
            catch (Exception ex)
            {
                
            }

            IsEmpty = Items.IsNullOrEmpty();
            SetProgressBar();
        }
    }
}
