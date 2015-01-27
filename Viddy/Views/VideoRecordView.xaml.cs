using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.System.Display;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Ioc;
using Viddy.ViewModel;

namespace Viddy.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class VideoRecordView
    {
        private DisplayRequest _displayRequest;
        private bool _isRecording;
        private readonly DisplayInformation _display;
        private readonly DispatcherTimer _recordingTimer;
        private readonly ICameraInfoService _cameraInfoService;
        private TimeSpan _recordedDuration;

        protected override ApplicationViewBoundsMode Mode
        {
            get { return ApplicationViewBoundsMode.UseCoreWindow; }
        }

        public static readonly DependencyProperty FlashOnProperty = DependencyProperty.Register(
            "FlashOn", typeof (bool), typeof (VideoRecordView), new PropertyMetadata(default(bool)));

        public bool FlashOn
        {
            get { return (bool) GetValue(FlashOnProperty); }
            set { SetValue(FlashOnProperty, value); }
        }

        public static readonly DependencyProperty IsFrontFacingProperty = DependencyProperty.Register(
            "IsFrontFacing", typeof (bool), typeof (VideoRecordView), new PropertyMetadata(default(bool)));

        public bool IsFrontFacing
        {
            get { return (bool) GetValue(IsFrontFacingProperty); }
            set { SetValue(IsFrontFacingProperty, value); }
        }
        
        public VideoRecordView()
        {
            InitializeComponent();
            _displayRequest = new DisplayRequest();
            
            _display = DisplayInformation.GetForCurrentView();
            _display.OrientationChanged += DisplayOnOrientationChanged;

            FlashViewbox.DataContext = this;
            FrontFacingViewbox.DataContext = this;

            _recordingTimer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(1)};
            _recordingTimer.Tick += RecordingTimerOnTick;

            _cameraInfoService = SimpleIoc.Default.GetInstance<ICameraInfoService>();
        }

        private void RecordingTimerOnTick(object sender, object o)
        {
            if (_recordedDuration == TimeSpan.MinValue)
            {
                _recordedDuration = TimeSpan.FromSeconds(0);
            }

            _recordedDuration = _recordedDuration.Add(TimeSpan.FromSeconds(1));
            var timeString = string.Format("{0:00}:{1:00}", _recordedDuration.Minutes, _recordedDuration.Seconds);
            Debug.WriteLine(timeString);
        }

        private void DisplayOnOrientationChanged(DisplayInformation sender, object args)
        {
            SetRotation(_display.CurrentOrientation);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            await StartPreview();

            Window.Current.VisibilityChanged += CurrentOnVisibilityChanged;
        }

        private async void CurrentOnVisibilityChanged(object sender, VisibilityChangedEventArgs e)
        {
            if (e.Visible)
            {
                await StartPreview();
            }
            else
            {
                StopVideo();
            }
        }

        private async Task StartPreview()
        {
            try
            {
                if (!_cameraInfoService.IsInitialised)
                {
                    var cameraType = IsFrontFacing ? CameraInfoService.CameraType.FrontFacing : CameraInfoService.CameraType.Primary;
                    await _cameraInfoService.StartService(cameraType);
                }
            }
            catch { }

            if (!_cameraInfoService.IsInitialised)
            {
                // TODO: Display error
                return;
            }

            await _cameraInfoService.StartPreview(PrePreviewTask());
        }

        private async Task PrePreviewTask()
        {
            var mediaCapture = _cameraInfoService.MediaCapture;

            var previewResolutions = mediaCapture.VideoDeviceController.GetAvailableMediaStreamProperties(MediaStreamType.VideoPreview).Cast<VideoEncodingProperties>().ToList();
            var recordingResolutions = mediaCapture.VideoDeviceController.GetAvailableMediaStreamProperties(MediaStreamType.VideoRecord).Cast<VideoEncodingProperties>().ToList();

            var highestPreviewRes = previewResolutions.FirstOrDefault(y => y.Height <= 720);
            var highestRecordingRes = recordingResolutions.FirstOrDefault(y => y.Height <= 720);

            await mediaCapture.VideoDeviceController.SetMediaStreamPropertiesAsync(MediaStreamType.VideoPreview, highestPreviewRes);
            await mediaCapture.VideoDeviceController.SetMediaStreamPropertiesAsync(MediaStreamType.VideoRecord, highestRecordingRes);

            CaptureElement.Source = _cameraInfoService.MediaCapture;
            SetRotation(_display.CurrentOrientation);
        }

        private async void SetRotation(DisplayOrientations orientation)
        {
            var mediaCapture = _cameraInfoService.MediaCapture;
            var rotation = VideoRotation.Clockwise90Degrees;
            if (orientation != _display.NativeOrientation || IsFrontFacing)
            {
                switch (orientation)
                {
                    case DisplayOrientations.Landscape:
                        rotation = IsFrontFacing ? VideoRotation.None : VideoRotation.Clockwise270Degrees;
                        break;
                    case DisplayOrientations.LandscapeFlipped:
                        rotation = VideoRotation.Clockwise180Degrees;
                        break;
                    default:
                        rotation = IsFrontFacing ? VideoRotation.Clockwise270Degrees : VideoRotation.Clockwise180Degrees;
                        break;
                }
            }

            if (_cameraInfoService.IsInitialised && !string.IsNullOrEmpty(mediaCapture.MediaCaptureSettings.VideoDeviceId) && !string.IsNullOrEmpty(mediaCapture.MediaCaptureSettings.AudioDeviceId))
            {
                //rotate the video feed according to the sensor
                mediaCapture.SetPreviewRotation(rotation);
                mediaCapture.SetRecordRotation(rotation);

                //hook into MediaCapture events
                mediaCapture.RecordLimitationExceeded += MediaCaptureOnRecordLimitationExceeded;
                mediaCapture.Failed += MediaCaptureOnFailed;

                //device initialized successfully
            }
            else
            {
                //no cam found
            }
        }

        private void MediaCaptureOnFailed(MediaCapture sender, MediaCaptureFailedEventArgs errorEventArgs)
        {
            var i = 1;
        }

        private void MediaCaptureOnRecordLimitationExceeded(MediaCapture sender)
        {
            var i = 1;
        }

        private string _fileName;
        private async void RecordButton_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            var mediaCapture = _cameraInfoService.MediaCapture;
            if (!_isRecording)
            {
                _isRecording = true;
                if (_displayRequest == null)
                {
                    _displayRequest = new DisplayRequest();
                }

                _displayRequest.RequestActive();

                _fileName = string.Format("Viddy-{0}.mp4", DateTime.Now.ToString("yyyy-M-dd-HH-mm-ss"));
                var folder = ApplicationData.Current.LocalCacheFolder;
                var file = await folder.CreateFileAsync(_fileName, CreationCollisionOption.ReplaceExisting);

                mediaCapture.StartRecordToStorageFileAsync(MediaEncodingProfile.CreateMp4(VideoEncodingQuality.Auto), file);
                _recordingTimer.Start();
            }
            else
            {
                _recordingTimer.Stop();
                _isRecording = false;
                _displayRequest.RequestRelease();
                _displayRequest = null;
                _recordedDuration = TimeSpan.MinValue;

                await mediaCapture.StopRecordAsync();

                if (string.IsNullOrEmpty(_fileName))
                {
                    return;
                }

                var folder = ApplicationData.Current.LocalCacheFolder;
                try
                {
                    var file = await folder.GetFileAsync(_fileName);
                    if (file != null)
                    {
                        var cameraRoll = KnownFolders.CameraRoll;
                        await file.MoveAsync(cameraRoll);

                        var movedFile = await cameraRoll.GetFileAsync(_fileName);

                        var vm = DataContext as VideoRecordViewModel;
                        if (vm != null)
                        {
                            vm.FinishedRecording(movedFile);
                        }
                    }
                }
                catch (FileNotFoundException)
                {
                    
                }
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            Window.Current.VisibilityChanged -= CurrentOnVisibilityChanged;
            StopVideo();
        }

        private void StopVideo()
        {
            
            if (_displayRequest != null && _isRecording)
            {
                _displayRequest.RequestRelease();
                _displayRequest = null;
            }

            if (!_cameraInfoService.IsInitialised)
            {
                return;
            }

            try
            {
                if (_isRecording)
                {
                    _cameraInfoService.MediaCapture.StopRecordAsync();
                }

                CaptureElement.Source = null;

                _cameraInfoService.DisposeMediaCapture();
            }
            catch { }
        }

        private void FlashButton_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            FlashOn = !FlashOn;
            TurnFlashOnOrOff(FlashOn);
        }

        private async void FrontFacingButton_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            IsFrontFacing = !IsFrontFacing;
            await _cameraInfoService.DisposeMediaCapture();
            await StartPreview();
        }

        private void TurnFlashOnOrOff(bool turnFlashOn)
        {
            if (!_cameraInfoService.IsInitialised)
            {
                return;
            }

            var mediaCapture = _cameraInfoService.MediaCapture;

            var torch = mediaCapture.VideoDeviceController.TorchControl;
            torch.Enabled = turnFlashOn;
        }
    }
}
