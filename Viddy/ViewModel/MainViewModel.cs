using System;
using System.Threading.Tasks;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using Viddy.Core.Services;
using Viddy.Views;
using Viddy.Views.Account;
using VidMePortable;
using VidMePortable.Model.Responses;

namespace Viddy.ViewModel
{
    public class MainViewModel : VideoLoadingViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IVidMeClient _vidMeClient;

        public MainViewModel(INavigationService navigationService, IVidMeClient vidMeClient)
        {
            _navigationService = navigationService;
            _vidMeClient = vidMeClient;

            if (IsInDesignMode)
            {
                IsEmpty = true;
            }
            else
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
            else
            {
                IsEmpty = true;
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
