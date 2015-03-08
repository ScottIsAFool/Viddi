using Windows.ApplicationModel.DataTransfer;
using GalaSoft.MvvmLight.Command;
using Viddi.Localisation;
using VidMePortable.Model;

namespace Viddi.ViewModel.Account.Manage
{
    public class OwnedAppViewModel : ViewModelBase
    {
        public Application Application { get; set; }

        private DataTransferManager _manager;

        public OwnedAppViewModel(Application application)
        {
            Application = application;
        }

        private void ManagerOnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            _manager.DataRequested -= ManagerOnDataRequested;
            var request = args.Request;
            request.Data.Properties.Title = string.Format(Resources.AppDetails, Application.Name);
            var message = string.Format("{0}: {1}\n{2}: {3}", Resources.NameText, Application.Name, Resources.ClientId, Application.ClientId);
            if (!string.IsNullOrEmpty(Application.ClientSecret))
            {
                message += string.Format("\n{0}: {1}", Resources.ClientSecret, Application.ClientSecret);
            }
            request.Data.Properties.Description = message;
            request.Data.SetText(message);
        }

        public RelayCommand ShareAppCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    _manager = DataTransferManager.GetForCurrentView();
                    _manager.DataRequested += ManagerOnDataRequested;
                    DataTransferManager.ShowShareUI();
                });
            }
        }
    }
}