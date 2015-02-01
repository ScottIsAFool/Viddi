using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Messaging;
using Viddy.Messaging;
using Viddy.ViewModel.Item;
using VidMePortable;

namespace Viddy.ViewModel
{
    public class ChannelViewModel : VideoLoadingViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IVidMeClient _vidMeClient;

        public ChannelViewModel(INavigationService navigationService, IVidMeClient vidMeClient)
        {
            _navigationService = navigationService;
            _vidMeClient = vidMeClient;
        }

        public ChannelItemViewModel Channel { get; set; }

        protected override void WireMessages()
        {
            Messenger.Default.Register<ChannelMessage>(this, m =>
            {
                Channel = m.Channel;
            });
        }
    }
}
