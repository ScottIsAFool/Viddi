using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Security.Authentication.Web;
using Windows.Storage;
using Windows.Storage.Pickers;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Viddy.Services;
using Viddy.Views;
using VidMePortable;
using VidMePortable.Model;
using VidMePortable.Model.Responses;

namespace Viddy.ViewModel
{
    public class AccountViewModel : ViewModelBase
    {
        private const string DefaultAvatar = "/Assets/Defaults/UserLoginDefault.png";

        private readonly INavigationService _navigationService;
        private readonly IVidMeClient _vidMeClient;

        private bool _videosLoaded;

        public AccountViewModel(INavigationService navigationService, IVidMeClient vidMeClient)
        {
            _navigationService = navigationService;
            _vidMeClient = vidMeClient;

            SetAvatar();
        }

        public ObservableCollection<Video> Videos { get; set; }
        public bool ChangingAvatar { get; set; }

        public RelayCommand LogInLogOutCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    if (AuthenticationService.Current.IsLoggedIn)
                    {
                        AuthenticationService.Current.SignOut();
                        _videosLoaded = false;
                        SetAvatar();
                        await LoadData(true);
                    }
                    else
                    {
                        LaunchAuthentication();
                    }
                });
            }
        }

        public RelayCommand PageLoadedCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await LoadData(false);
                });
            }
        }

        public RelayCommand RefreshCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await LoadData(true);
                });
            }
        }

        public RelayCommand ChangeAvatarCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    var filePicker = new FileOpenPicker {ViewMode = PickerViewMode.Thumbnail, SuggestedStartLocation = PickerLocationId.PicturesLibrary};
                    filePicker.FileTypeFilter.Add(".jpg");
                    filePicker.FileTypeFilter.Add(".jpeg");
                    filePicker.FileTypeFilter.Add(".png");

                    filePicker.PickSingleFileAndContinue();
                });
            }
        }

        public RelayCommand<Video> DeleteCommand
        {
            get
            {
                return new RelayCommand<Video>(async video =>
                {
                    if (video == null)
                    {
                        return;
                    }

                    try
                    {
                        if (AuthenticationService.Current.IsLoggedIn)
                        {
                            if (await _vidMeClient.DeleteVideoAsync(video.VideoId))
                            {
                                Videos.Remove(video);
                            }
                        }
                        else
                        {
                            if (await _vidMeClient.DeleteAnonymousVideoAsync(video.VideoId, ""))
                            {
                                Videos.Remove(video);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        
                    }
                });
            }
        }

        public RelayCommand NavigateToSettingsCommand
        {
            get
            {
                return new RelayCommand(() => _navigationService.Navigate<SettingsView>());
            }
        }

        public RelayCommand NavigateToEditProfileCommand
        {
            get { return new RelayCommand(() => _navigationService.Navigate<EditProfileView>()); }
        }

        private void SetAvatar()
        {
            AvatarUrl = AuthenticationService.Current.IsLoggedIn ? AuthenticationService.Current.AuthenticationInfo.User.AvatarUrl : DefaultAvatar;
        }

        public string AvatarUrl { get; set; }

        private void LaunchAuthentication()
        {
            var url = _vidMeClient.GetAuthUrl(Constants.ClientId, Constants.CallBackUrl, new List<Scope> {Scope.Videos, Scope.VideoUpload, Scope.Channels, Scope.Comments, Scope.Votes, Scope.Account, Scope.AuthManagement, Scope.ClientManagement});

            WebAuthenticationBroker.AuthenticateAndContinue(new Uri(url), new Uri(Constants.CallBackUrl), new ValueSet(), WebAuthenticationOptions.None);
        }

        private async Task CompleteAuthentication(string code)
        {
            try
            {
                var auth = await _vidMeClient.ExchangeCodeForTokenAsync(code, Constants.ClientId, Constants.ClientSecret);
                if (auth != null)
                {
                    AuthenticationService.Current.SetAuthenticationInfo(auth);
                }

                SetAvatar();
                await LoadData(true);
            }
            catch (Exception ex)
            {
                
            }
        }

        private async Task LoadData(bool isRefresh)
        {
            if (_videosLoaded && !isRefresh)
            {
                return;
            }

            try
            {
                SetProgressBar("Getting videos...");
                VideosResponse response;
                if (AuthenticationService.Current.IsLoggedIn)
                {
                    response = await _vidMeClient.GetUserVideosAsync(AuthenticationService.Current.LoggedInUserId);
                }
                else
                {
                    response = await _vidMeClient.GetAnonymousVideosAsync();
                }

                if (Videos != null)
                {
                    Videos.Clear();
                }

                Videos = new ObservableCollection<Video>(response.Videos);
                _videosLoaded = true;
            }
            catch (Exception ex)
            {
                
            }

            SetProgressBar();
        }

        protected override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, async m =>
            {
                if (m.Notification.Equals(Constants.Messages.AuthCodeMsg))
                {
                    var code = (string) m.Sender;
                    await CompleteAuthentication(code);
                }
                else if (m.Notification.Equals(Constants.Messages.ProfileFileMsg))
                {
                    var file = m.Sender as IStorageFile;
                    await UpdateAvatar(file);
                }
            });
        }

        private async Task UpdateAvatar(IStorageFile file)
        {
            try
            {
                ChangingAvatar = true;

                using (var stream = await file.OpenAsync(FileAccessMode.Read))
                {
                    using (var actualStream = stream.AsStream())
                    {
                        var user = await _vidMeClient.UpdateAvatarAsync(AuthenticationService.Current.LoggedInUserId, actualStream, file.ContentType, file.Name);
                        if (user != null)
                        {
                            var auth = AuthenticationService.Current.AuthenticationInfo;
                            auth.User = user;
                            AuthenticationService.Current.SetAuthenticationInfo(auth);
                            SetAvatar();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                
            }

            ChangingAvatar = false;
        }
    }
}
