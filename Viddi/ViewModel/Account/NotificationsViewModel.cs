using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Cimbalino.Toolkit.Extensions;
using ScottIsAFool.Windows.Core.Extensions;
using Viddi.Core.Extensions;
using Viddi.Services;
using Viddi.ViewModel.Item;
using VidMePortable;

namespace Viddi.ViewModel.Account
{
    public class NotificationsViewModel : LoadingItemsViewModel<NotificationItemViewModel>
    {
        private readonly IVidMeClient _vidMeClient;
        private readonly INotificationService _notificationService;

        public NotificationsViewModel(IVidMeClient vidMeClient, INotificationService notificationService)
        {
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

            HasErrors = false;

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
                    CanLoadMore = Items != null && Items.Count < response.Page.Total;

                    ItemsLoaded = true;
                }
            }
            catch (Exception ex)
            {
                Log.ErrorException("LoadData()", ex);
                HasErrors = true;
            }

            SetProgressBar();
        }
    }
}
