using System;
using System.IO;
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
using Viddy.ViewModel;

namespace Viddy.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        private MediaCapture _mediaCapture;
        private DisplayRequest _displayRequest;
        private bool _isRecording;
        private readonly DisplayInformation _display;

        protected override ApplicationViewBoundsMode Mode
        {
            get { return ApplicationViewBoundsMode.UseCoreWindow; }
        }

        public static readonly DependencyProperty FlashOnProperty = DependencyProperty.Register(
            "FlashOn", typeof (bool), typeof (MainPage), new PropertyMetadata(default(bool)));

        public bool FlashOn
        {
            get { return (bool) GetValue(FlashOnProperty); }
            set { SetValue(FlashOnProperty, value); }
        }

        public static readonly DependencyProperty IsFrontFacingProperty = DependencyProperty.Register(
            "IsFrontFacing", typeof (bool), typeof (MainPage), new PropertyMetadata(default(bool)));

        public bool IsFrontFacing
        {
            get { return (bool) GetValue(IsFrontFacingProperty); }
            set { SetValue(IsFrontFacingProperty, value); }
        }
        
        public MainPage()
        {
            InitializeComponent();
            _displayRequest = new DisplayRequest();
            
            _display = DisplayInformation.GetForCurrentView();
            _display.OrientationChanged += DisplayOnOrientationChanged;

            FlashViewbox.DataContext = this;
            FrontFacingViewbox.DataContext = this;
        }

        private void DisplayOnOrientationChanged(DisplayInformation sender, object args)
        {
            SetRotation(_display.CurrentOrientation);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

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
            if (_mediaCapture == null)
            {
                _mediaCapture = new MediaCapture();
            }

            await _mediaCapture.InitializeAsync();

            SetRotation(_display.CurrentOrientation);

            CaptureElement.Source = _mediaCapture;
            await _mediaCapture.StartPreviewAsync();
        }

        private void SetRotation(DisplayOrientations orientation)
        {
            var rotation = VideoRotation.Clockwise90Degrees;
            if (orientation != _display.NativeOrientation)
            {
                switch (orientation)
                {
                    case DisplayOrientations.Landscape:
                        rotation = VideoRotation.Clockwise270Degrees;
                        break;
                    case DisplayOrientations.LandscapeFlipped:
                        rotation = VideoRotation.Clockwise180Degrees;
                        break;
                    default:
                        rotation = VideoRotation.Clockwise180Degrees;
                        break;
                }
            }

            if (!string.IsNullOrEmpty(_mediaCapture.MediaCaptureSettings.VideoDeviceId) && !string.IsNullOrEmpty(_mediaCapture.MediaCaptureSettings.AudioDeviceId))
            {
                //rotate the video feed according to the sensor
                _mediaCapture.SetPreviewRotation(rotation);
                _mediaCapture.SetRecordRotation(rotation);

                //hook into MediaCapture events
                _mediaCapture.RecordLimitationExceeded += MediaCaptureOnRecordLimitationExceeded;
                _mediaCapture.Failed += MediaCaptureOnFailed;

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

        private async void RecordButton_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            return;
            var fileName = string.Empty;
            if (!_isRecording)
            {
                _isRecording = true;
                if (_displayRequest == null)
                {
                    _displayRequest = new DisplayRequest();
                }

                _displayRequest.RequestActive();

                fileName = "Viddy-" + DateTime.Now.ToString("yyyy-M-dd-HH-mm-ss") + ".mp4";
                var folder = ApplicationData.Current.LocalCacheFolder;
                var file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

                _mediaCapture.StartRecordToStorageFileAsync(new MediaEncodingProfile(), file);
            }
            else
            {
                _isRecording = false;
                _displayRequest.RequestRelease();
                _displayRequest = null;

                await _mediaCapture.StopRecordAsync();

                if (string.IsNullOrEmpty(fileName))
                {
                    return;
                }

                var folder = ApplicationData.Current.LocalCacheFolder;
                try
                {
                    var file = await folder.GetFileAsync(fileName);
                    if (file != null)
                    {
                        var cameraRoll = await KnownFolders.PicturesLibrary.GetFolderAsync("Camera Roll");
                        var copiedFile = await file.CopyAsync(cameraRoll);

                        var vm = DataContext as MainViewModel;
                        if (vm != null)
                        {
                            vm.FinishedRecording(file);
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

            if (_mediaCapture == null)
            {
                return;
            }

            _mediaCapture.StopPreviewAsync();
            
            if (_isRecording)
            {
                _mediaCapture.StopRecordAsync();
            }

            CaptureElement.Source = null;

            _mediaCapture.Dispose();
            _mediaCapture = null;
        }

        private void FlashButton_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            FlashOn = !FlashOn;
        }

        private void FrontFacingButton_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            IsFrontFacing = !IsFrontFacing;
        }
    }
}
