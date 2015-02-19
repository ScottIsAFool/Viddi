using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Store;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using Viddy.Services;

namespace Viddy.ViewModel
{
    public class AboutViewModel : ViewModelBase
    {
        private readonly ILauncherService _launcherService;
        private readonly IEmailComposeService _emailCompose;
        private readonly ReviewService _reviewService;
        private readonly PackageVersion _version;

        public AboutViewModel(ILauncherService launcherService, IEmailComposeService emailCompose, ReviewService reviewService)
        {
            _launcherService = launcherService;
            _emailCompose = emailCompose;
            _reviewService = reviewService;
            _version = Package.Current.Id.Version;
        }

        public string Version
        {
            get { return string.Format("{0}.{1}.{2}", _version.Major, _version.Minor, _version.Build); }
        }

        public RelayCommand ViewLoadedCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    var manager = DataTransferManager.GetForCurrentView();
                    manager.DataRequested += ManagerOnDataRequested;
                });
            }
        }

        public RelayCommand LeaveReviewCommand
        {
            get
            {
                return new RelayCommand(() => _reviewService.LaunchReview());
            }
        }

        public RelayCommand ScottTwitterCommand
        {
            get
            {
                return new RelayCommand(() => _launcherService.LaunchUriAsync(new Uri("http://twitter.com/scottisafool", UriKind.Absolute)));
            }
        }

        public RelayCommand EmailCommand
        {
            get
            {
                return new RelayCommand(() => _emailCompose.ShowAsync("scottisafool@live.co.uk", "Feedback from Viddy", string.Empty));
            }
        }

        public RelayCommand TellAFriendCommand
        {
            get
            {
                return new RelayCommand(DataTransferManager.ShowShareUI);
            }
        }

        private void ManagerOnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var request = args.Request;
            request.Data.Properties.Title = "Check out Viddy";

            var messageTemplate = "Viddy is an app that lets you browse and upload videos to VidMe\n\n{0}";
            var message = string.Format(messageTemplate, CurrentApp.LinkUri);

            request.Data.Properties.Description = message;
            request.Data.SetText(message);
        }
    }
}
