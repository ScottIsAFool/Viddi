using System;
using System.Collections.Generic;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Messaging;
using ScottIsAFool.Windows.Core.Extensions;
using Viddi.Core;
using Viddi.Core.Extensions;
using Viddi.Messaging;
using Viddi.ViewModel.Item;
using Viddi.Views.Account;

namespace Viddi.ViewModel.Account
{
    public class UserFollowersViewModel : ViewModelBase, IBackSupportedViewModel
    {
        private readonly INavigationService _navigationService;

        public UserFollowersViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        private Stack<ICanShowFollowers> _previousItems;

        public ICanShowFollowers Item { get; set; }

        protected override void WireMessages()
        {
            Messenger.Default.Register<UserMessage>(this, m =>
            {
                if (m.Notification.Equals(Constants.Messages.UserDetailMsg))
                {
                    var user = Item as UserViewModel;
                    if (user == null || user.Id != m.User.Id)
                    {
                        Item = m.User;
                        Item.Followers.Id = Item.Id;
                    }
                }
            });

            Messenger.Default.Register<ChannelMessage>(this, m =>
            {
                if (m.Notification.Equals(Constants.Messages.UserDetailMsg))
                {
                    var channel = Item as ChannelItemViewModel;
                    if (channel == null || channel.Id != m.Channel.Id)
                    {
                        Item = m.Channel;
                        Item.Followers.Id = Item.Id;
                    }
                }
            });
        }

        #region IBackSupportedViewModel implementations
        public void ChangeContext(Type callingType)
        {
            if (_previousItems.IsNullOrEmpty() || callingType != typeof(UserFollowersView))
            {
                return;
            }

            var item = _previousItems.Pop();
            if (item != null)
            {
                Item = item;
            }
        }

        public void SaveContext()
        {
            if (_previousItems == null)
            {
                _previousItems = new Stack<ICanShowFollowers>();
            }

            _previousItems.Push(Item);
        }
        #endregion
    }
}
