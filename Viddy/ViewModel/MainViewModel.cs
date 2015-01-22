using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
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
    public class MainViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly ICameraInfoService _cameraInfo;


        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(INavigationService navigationService, ICameraInfoService cameraInfo)
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
                SetDeviceOptions();
            }
        }

        private async Task SetDeviceOptions()
        {
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