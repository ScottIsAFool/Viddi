using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using GalaSoft.MvvmLight.Messaging;
using Viddy.Services;
using VidMePortable;

namespace Viddy.ViewModel
{
    public class AvatarViewModel : ViewModelBase
    {
        private const string DefaultAvatar = "/Assets/Defaults/UserLoginDefault.png";
        private readonly IVidMeClient _vidMeClient;

        public AvatarViewModel(IVidMeClient vidMeClient)
        {
            _vidMeClient = vidMeClient;

            AuthenticationService.Current.UserSignedIn += UserStateChanged;
            AuthenticationService.Current.UserSignedOut += UserStateChanged;
        }

        private void UserStateChanged(object sender, EventArgs eventArgs)
        {
        }

        public void ChangeAvatar()
        {
            var filePicker = new FileOpenPicker { ViewMode = PickerViewMode.Thumbnail, SuggestedStartLocation = PickerLocationId.PicturesLibrary };
            filePicker.FileTypeFilter.Add(".jpg");
            filePicker.FileTypeFilter.Add(".jpeg");
            filePicker.FileTypeFilter.Add(".png");

            filePicker.PickSingleFileAndContinue();
        }

        public bool ChangingAvatar { get; set; }

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
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            ChangingAvatar = false;
        }

        protected override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, async m =>
            {
                if (m.Notification.Equals(Constants.Messages.ProfileFileMsg))
                {
                    var file = m.Sender as IStorageFile;
                    await UpdateAvatar(file);
                }
            });
        }
    }
}
