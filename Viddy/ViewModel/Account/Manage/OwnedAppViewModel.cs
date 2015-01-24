using Windows.ApplicationModel.DataTransfer;
using GalaSoft.MvvmLight.Command;
using VidMePortable;
using VidMePortable.Model;

namespace Viddy.ViewModel.Account.Manage
{
    public class OwnedAppViewModel : ViewModelBase
    {
        private readonly IVidMeClient _vidMeClient;
        public Application Application { get; set; }

        private readonly DataTransferManager _manager;

        public OwnedAppViewModel(Application application, IVidMeClient vidMeClient)
        {
            _vidMeClient = vidMeClient;
            Application = application;

            _manager = DataTransferManager.GetForCurrentView();
        }

        private void ManagerOnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            _manager.DataRequested -= ManagerOnDataRequested;
            var request = args.Request;
            request.Data.Properties.Title = Application.Name + " Details";
            var message = string.Format("Name: {0}\nClient ID: {1}", Application.Name, Application.ClientId);
            request.Data.Properties.Description = message;
            request.Data.SetText(message);
        }

        public RelayCommand ShareAppCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    _manager.DataRequested += ManagerOnDataRequested;
                    DataTransferManager.ShowShareUI();
                });
            }
        }
    }
}