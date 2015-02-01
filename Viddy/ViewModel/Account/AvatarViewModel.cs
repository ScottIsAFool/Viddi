using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using GalaSoft.MvvmLight.Messaging;
using Viddy.Services;
using VidMePortable;
using VidMePortable.Model;

namespace Viddy.ViewModel.Account
{
    public class AvatarViewModel : ViewModelBase
    {
        private const string DefaultAvatar = "/Assets/Defaults/UserLoginDefault.png";
        private readonly IVidMeClient _vidMeClient;

        private PictureType _pictureType;

        public AvatarViewModel(IVidMeClient vidMeClient)
        {
            _vidMeClient = vidMeClient;
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

        private static void LaunchFilePicker()
        {
            var filePicker = new FileOpenPicker {ViewMode = PickerViewMode.Thumbnail, SuggestedStartLocation = PickerLocationId.PicturesLibrary};
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

                using (var stream = await file.OpenAsync(FileAccessMode.Read))
                {
                    using (var actualStream = stream.AsStream())
                    {
                        var user = await SendPictureData(AuthenticationService.Current.LoggedInUserId, actualStream, file.ContentType, file.Name);
                        if (user != null)
                        {
                            var auth = AuthenticationService.Current.AuthenticationInfo;
                            auth.User = user;
                            AuthenticationService.Current.SetAuthenticationInfo(auth);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            SetChanging(false);
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
        }

        private enum PictureType
        {
            Avatar,
            Cover
        }
    }
}
