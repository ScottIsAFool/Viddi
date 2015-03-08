using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Email;
using Windows.ApplicationModel.Store;
using Windows.Storage;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using ScottIsAFool.Windows.Core.Logging;
using Viddi.Localisation;
using Viddi.Services;
using Viddi.Views;

namespace Viddi.ViewModel
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
                return new RelayCommand(async () =>
                {
                    await EmailLogs();
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
                return new RelayCommand(() => _emailCompose.ShowAsync("scottisafool@live.co.uk", "Feedback from Viddi", string.Empty));
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
            }
        }

        private async Task EmailLogs()
        {
            try
            {
                var email = new EmailMessage();
                email.To.Add(new EmailRecipient("scottisafool@live.co.uk", "Scott Lovegrove"));
                email.Subject = "Logfile from Viddi (" + Version + "." + _version.Revision + ")";

                var folder = ApplicationData.Current.LocalFolder;
                var file = await folder.GetFileAsync(WinLogger.LogFileName);

                if (file != null)
                {
                    email.Attachments.Add(new EmailAttachment("LogFile.txt", file));
                    await EmailManager.ShowComposeNewEmailAsync(email);
                }
            }
            catch
            {
            }
        }

        private void TellAFriend(DataRequest request)
        {
            request.Data.Properties.Title = Resources.TellAFriendTitle;

            var messageTemplate = Resources.TellAFriendBody;
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
