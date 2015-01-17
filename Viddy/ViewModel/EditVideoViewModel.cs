using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Messaging;
using VidMePortable;

namespace Viddy.ViewModel
{
    public class EditVideoViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IVidMeClient _vidMeClient;

        public EditVideoViewModel(INavigationService navigationService, IVidMeClient vidMeClient)
        {
            _navigationService = navigationService;
            _vidMeClient = vidMeClient;
        }

        protected override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, m =>
            {
                
            });
        }
    }
}
