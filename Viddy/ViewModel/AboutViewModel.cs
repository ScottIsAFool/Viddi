using System;
using System.Collections.Generic;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Store;
using Windows.Storage;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using ScottIsAFool.WindowsPhone.Logging;
using Viddy.Services;
using Viddy.Views;

namespace Viddy.ViewModel
{
    public class AboutViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly ILauncherService _launcherService;
        private readonly IEmailComposeService _emailCompose;
        private readonly ReviewService _reviewService;
        private readonly PackageVersion _version;

        private ShareType _shareType;

        public AboutViewModel(INavigationService navigationService, ILauncherService launcherService, IEmailComposeService emailCompose, ReviewService reviewService)
        {
            _navigationService = navigationService;
            _launcherService = launcherService;
            _emailCompose = emailCompose;
            _reviewService = reviewService;
            _version = Package.Current.Id.Version;
        }

        public string Version
        {
            get { return string.Format("{0}.{1}.{2}", _version.Major, _version.Minor, _version.Build); }
        }

        public bool CanSendLogs
        {
            get { return true; }
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

        public RelayCommand EmailLogsCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    _shareType = ShareType.EmailLogs;
                    DataTransferManager.ShowShareUI();
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

        public RelayCommand DeaniTwitterCommand
        {
            get
            {
                return new RelayCommand(() => _launcherService.LaunchUriAsync(new Uri("http://twitter.com/deanihansen", UriKind.Absolute)));
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
                return new RelayCommand(() =>
                {
                    _shareType = ShareType.TellAFriend;
                    DataTransferManager.ShowShareUI();
                });
            }
        }

        public RelayCommand NavigateToToolsCommand
        {
            get { return new RelayCommand(() => _navigationService.Navigate<ToolsUsedView>()); }
        }

        private void ManagerOnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var request = args.Request;
            switch (_shareType)
            {
                case ShareType.TellAFriend:
                    TellAFriend(request);
                    break;
                case ShareType.EmailLogs:
                    EmailLogs(request);
                    break;
            }
        }

        private async void EmailLogs(DataRequest request)
        {
            var deferral = request.GetDeferral();

            request.Data.Properties.Title = "Logfile from Viddy " + Version;

            var folder = ApplicationData.Current.LocalFolder;
            var file = await folder.GetFileAsync(WinLogger.LogFileName);

            if (file != null)
            {
                request.Data.SetStorageItems(new List<IStorageFile> { file });
            }

            deferral.Complete();
        }

        private static void TellAFriend(DataRequest request)
        {
            request.Data.Properties.Title = "Check out Viddy";

            var messageTemplate = "Viddy is an app that lets you browse and upload videos to VidMe\n\n{0}";
            var message = string.Format(messageTemplate, CurrentApp.LinkUri);

            request.Data.Properties.Description = message;
            request.Data.SetText(message);
        }

        private enum ShareType
        {
            EmailLogs,
            TellAFriend
        }
    }
}
