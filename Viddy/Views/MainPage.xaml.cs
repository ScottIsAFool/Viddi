using System;
using Windows.Devices.Sensors;
using Windows.Media.Capture;
using Windows.System.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

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

        public MainPage()
        {
            InitializeComponent();
            _mediaCapture = new MediaCapture();
            _displayRequest = new DisplayRequest();

            SetFullScreen(ApplicationViewBoundsMode.UseCoreWindow);
            var sensor = new SimpleOrientationSensor();
            sensor.OrientationChanged += SensorOnOrientationChanged;
        }

        private void SensorOnOrientationChanged(SimpleOrientationSensor sender, SimpleOrientationSensorOrientationChangedEventArgs args)
        {
        
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            await _mediaCapture.InitializeAsync();
            
            SetRotation();

            CaptureElement.Source = _mediaCapture;
            await _mediaCapture.StartPreviewAsync();
        }

        private void SetRotation()
        {
            if (!string.IsNullOrEmpty(_mediaCapture.MediaCaptureSettings.VideoDeviceId) && !string.IsNullOrEmpty(_mediaCapture.MediaCaptureSettings.AudioDeviceId))
            {
                //rotate the video feed according to the sensor
                _mediaCapture.SetPreviewRotation(VideoRotation.Clockwise270Degrees);
                _mediaCapture.SetRecordRotation(VideoRotation.Clockwise270Degrees);

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
        }

        private void MediaCaptureOnRecordLimitationExceeded(MediaCapture sender)
        {
        }

        private void RecordButton_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            if (!_isRecording)
            {
                _isRecording = true;
                if (_displayRequest == null)
                {
                    _displayRequest = new DisplayRequest();
                }

                _displayRequest.RequestActive();
            }
            else
            {
                _isRecording = false;
                _displayRequest.RequestRelease();
                _displayRequest = null;
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            if (_displayRequest != null)
            {
                _displayRequest.RequestRelease();
                _displayRequest = null;
            }
        }
    }
}
