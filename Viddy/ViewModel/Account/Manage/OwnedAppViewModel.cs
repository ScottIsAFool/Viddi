using Windows.ApplicationModel.DataTransfer;
using GalaSoft.MvvmLight.Command;
using Viddy.Core.Services;
using VidMePortable.Model;

namespace Viddy.ViewModel.Account.Manage
{
    public class OwnedAppViewModel : ViewModelBase
    {
        private readonly ILocalisationLoader _localisationLoader;
        public Application Application { get; set; }

        private readonly DataTransferManager _manager;

        public OwnedAppViewModel(Application application, ILocalisationLoader localisationLoader)
        {
            _localisationLoader = localisationLoader;
            Application = application;

            _manager = DataTransferManager.GetForCurrentView();
        }

        private void ManagerOnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            _manager.DataRequested -= ManagerOnDataRequested;
            var request = args.Request;
            request.Data.Properties.Title = string.Format(_localisationLoader.GetString("AppDetails"), Application.Name);
            var message = string.Format("{0}: {1}\n{2}: {3}", _localisationLoader.GetString("NameText"), Application.Name, _localisationLoader.GetString("ClientId"), Application.ClientId);
            if (!string.IsNullOrEmpty(Application.ClientSecret))
            {
                message += string.Format("\n{0}: {1}", _localisationLoader.GetString("ClientSecret"), Application.ClientSecret);
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
                    _manager.DataRequested += ManagerOnDataRequested;
                    DataTransferManager.ShowShareUI();
                });
            }
        }
    }
}