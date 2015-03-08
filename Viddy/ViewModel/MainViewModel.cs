using System;
using System.Threading.Tasks;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using Viddi.Core.Services;
using Viddi.Views;
using Viddi.Views.Account;
using VidMePortable;
using VidMePortable.Model.Responses;

namespace Viddi.ViewModel
{
    public class MainViewModel : VideoLoadingViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IVidMeClient _vidMeClient;

        public MainViewModel(INavigationService navigationService, IVidMeClient vidMeClient)
        {
            _navigationService = navigationService;
            _vidMeClient = vidMeClient;

            if (!IsInDesignMode)
            {
                AuthenticationService.Current.UserSignedOut += UserStateChanged;
                AuthenticationService.Current.UserSignedIn += UserStateChanged;
            }
        }

        private void UserStateChanged(object sender, EventArgs eventArgs)
        {
            Reset();
        }

        public override async Task PageLoaded()
        {
            if (AuthenticationService.Current.IsLoggedIn)
            {
                await base.PageLoaded();
            }
        }

        public override Task<VideosResponse> GetVideos(int offset)
        {
            return _vidMeClient.GetUserFeedAsync(offset);
        }

        public RelayCommand NavigateToAccountCommand
        {
            get { return new RelayCommand(() => _navigationService.Navigate<AccountView>()); }
        }

        public RelayCommand NavigateToVideoRecordCommand
        {
            get { return new RelayCommand(() => _navigationService.Navigate<VideoRecordView>()); }
        }

        public RelayCommand NavigateToSearchCommand
        {
            get { return new RelayCommand(() => _navigationService.Navigate<SearchView>()); }
        }

        public RelayCommand NavigateToBrowseChannelsCommand
        {
            get { return new RelayCommand(() => _navigationService.Navigate<BrowseChannelsView>()); }
        }

        public RelayCommand NavigateToSettingsCommand
        {
            get { return new RelayCommand(() => _navigationService.Navigate<SettingsView>()); }
        }

        public RelayCommand NavigateToAboutCommand
        {
            get { return new RelayCommand(() => _navigationService.Navigate<AboutView>()); }
        }
    }
}
