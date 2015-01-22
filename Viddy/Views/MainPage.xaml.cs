using System;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.Media.Capture;
using Windows.System.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight.Messaging;

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

        public MainPage()
        {
            InitializeComponent();
            _mediaCapture = new MediaCapture();
            _displayRequest = new DisplayRequest();

            SetFullScreen(ApplicationViewBoundsMode.UseCoreWindow);
            _display = DisplayInformation.GetForCurrentView();
            _display.OrientationChanged += DisplayOnOrientationChanged;

            Messenger.Default.Register<NotificationMessage>(this, m =>
            {
                if (m.Notification.Equals(Constants.Messages.AppLaunchedMsg))
                {
                    //StartPreview();
                }
            });
        }

        private void DisplayOnOrientationChanged(DisplayInformation sender, object args)
        {
            SetRotation(_display.CurrentOrientation);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            //await StartPreview();
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
                        rotation = VideoRotation.Clockwise90Degrees;
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
            StopVideo();
        }

        private void StopVideo()
        {
            if (_displayRequest != null)
            {
                _displayRequest.RequestRelease();
                _displayRequest = null;
            }

            _mediaCapture.StopPreviewAsync();

            if (_isRecording)
            {
                _mediaCapture.StopRecordAsync();
            }

            _mediaCapture.Dispose();
            _mediaCapture = null;
        }
    }
}
