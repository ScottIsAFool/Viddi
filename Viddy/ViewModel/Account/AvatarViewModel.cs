using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Viddy.Core.Services;
using Viddy.Services;
using VidMePortable;
using VidMePortable.Model;

namespace Viddy.ViewModel.Account
{
    public class AvatarViewModel : ViewModelBase
    {
        private const string DefaultAvatar = "/Assets/Defaults/UserLoginDefault.png";
        private readonly IVidMeClient _vidMeClient;
        private readonly IToastService _toastService;

        private PictureType _pictureType;

        public AvatarViewModel(IVidMeClient vidMeClient, IToastService toastService)
        {
            _vidMeClient = vidMeClient;
            _toastService = toastService;
        }

        public void ChangeAvatar()
        {
            _pictureType = PictureType.Avatar;

            LaunchFilePicker();
        }

        public void ChangeCover()
        {
            _pictureType = PictureType.Cover;

            LaunchFilePicker();
        }

        public RelayCommand ChangeAvatarCommand
        {
            get { return new RelayCommand(ChangeAvatar); }
        }

        public RelayCommand ChangeCoverCommand
        {
            get { return new RelayCommand(ChangeCover); }
        }

        public RelayCommand RemoveAvatarCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    try
                    {
                        ChangingAvatar = true;
                        var user = await _vidMeClient.RemoveAvatarAsync(AuthenticationService.Current.LoggedInUserId);
                        UpdateUserInfo(user);
                    }
                    catch (Exception ex)
                    {
                        
                    }

                    ChangingAvatar = false;
                });
            }
        }

        public RelayCommand RemoveCoverCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    try
                    {
                        ChangingCover = true;
                        var user = await _vidMeClient.RemoveCoverAsync(AuthenticationService.Current.LoggedInUserId);
                        UpdateUserInfo(user);
                    }
                    catch (Exception ex)
                    {

                    }

                    ChangingCover = false;
                });
            }
        }

        private static void LaunchFilePicker()
        {
            var filePicker = new FileOpenPicker { ViewMode = PickerViewMode.Thumbnail, SuggestedStartLocation = PickerLocationId.PicturesLibrary };
            filePicker.FileTypeFilter.Add(".jpg");
            filePicker.FileTypeFilter.Add(".jpeg");
            filePicker.FileTypeFilter.Add(".png");

            filePicker.PickSingleFileAndContinue();
        }

        public bool ChangingAvatar { get; set; }
        public bool ChangingCover { get; set; }

        private async Task UpdatePicture(IStorageFile file)
        {
            try
            {
                SetChanging(true);

                if (_pictureType == PictureType.Cover)
                {
                    var storageFile = file as StorageFile;
                    if (storageFile != null)
                    {
                        var properties = await storageFile.Properties.GetImagePropertiesAsync();
                        if (properties != null)
                        {
                            if (properties.Width < 800 || properties.Height < 600)
                            {
                                _toastService.Show("Image too small");
                                return;
                            }
                        }
                    }
                }

                using (var stream = await file.OpenAsync(FileAccessMode.Read))
                {
                    using (var actualStream = stream.AsStream())
                    {
                        var user = await SendPictureData(AuthenticationService.Current.LoggedInUserId, actualStream, file.ContentType, file.Name);
                        UpdateUserInfo(user);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            SetChanging(false);
        }

        private static void UpdateUserInfo(User user)
        {
            if (user != null)
            {
                var auth = AuthenticationService.Current.AuthenticationInfo;
                auth.User = user;
                AuthenticationService.Current.SetAuthenticationInfo(auth);
            }
        }

        private void SetChanging(bool value)
        {
            if (_pictureType == PictureType.Avatar)
            {
                ChangingAvatar = value;
            }
            else
            {
                ChangingCover = value;
            }
        }

        private Task<User> SendPictureData(string loggedInUserId, Stream actualStream, string contentType, string name)
        {
            return _pictureType == PictureType.Avatar
                ? _vidMeClient.UpdateAvatarAsync(loggedInUserId, actualStream, contentType, name)
                : _vidMeClient.UpdateCoverAsync(loggedInUserId, actualStream, contentType, name);
        }

        protected override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, async m =>
            {
                if (m.Notification.Equals(Constants.Messages.ProfileFileMsg))
                {
                    var file = m.Sender as IStorageFile;

                    await UpdatePicture(file);
                }
            });

            base.WireMessages();
        }

        private enum PictureType
        {
            Avatar,
            Cover
        }
    }
}
