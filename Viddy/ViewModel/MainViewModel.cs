using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Viddy.Views;

namespace Viddy.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class VideoRecordViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly ICameraInfoService _cameraInfo;


        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public VideoRecordViewModel(INavigationService navigationService, ICameraInfoService cameraInfo)
        {
            _navigationService = navigationService;
            _cameraInfo = cameraInfo;
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
                CanTurnOnFlash = true;
                HasFrontFacingCamera = true;
            }
            else
            {
                // Code runs "for real"
            }
        }

        private async Task SetDeviceOptions()
        {
            await _cameraInfo.StartService();
            CanTurnOnFlash = await _cameraInfo.HasFlash();
            HasFrontFacingCamera = await _cameraInfo.HasFrontFacingCamera();
        }

        public bool CanTurnOnFlash { get; set; }
        public bool HasFrontFacingCamera { get; set; }

        public RelayCommand MainPageLoadedCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    SetDeviceOptions();
                });
            }
        }

        public RelayCommand NavigateToAccountCommand
        {
            get
            {
                return new RelayCommand(() => _navigationService.Navigate<AccountView>());
            }
        }

        public void FinishedRecording(IStorageFile file)
        {
            if (App.Locator.Upload != null)
            {
                Messenger.Default.Send(new NotificationMessage(file, Constants.Messages.VideoFileMsg));
                SimpleIoc.Default.GetInstance<INavigationService>().Navigate<UploadVideoView>();
            }
        }

        public RelayCommand SelectVideoCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    var filePicker = new FileOpenPicker { ViewMode = PickerViewMode.Thumbnail, SuggestedStartLocation = PickerLocationId.VideosLibrary };
                    filePicker.FileTypeFilter.Add(".avi");
                    filePicker.FileTypeFilter.Add(".mov");
                    filePicker.FileTypeFilter.Add(".mp4");
                    filePicker.FileTypeFilter.Add(".wmv");

                    filePicker.PickSingleFileAndContinue();
                });
            }
        }
    }
}