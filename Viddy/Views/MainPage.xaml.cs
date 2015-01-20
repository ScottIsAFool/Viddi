using System;
using Windows.Media.Capture;
using Windows.UI.Xaml.Navigation;

namespace Viddy.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        private MediaCapture _mediaCapture;

        public MainPage()
        {
            this.InitializeComponent();
            _mediaCapture = new MediaCapture();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //await _mediaCapture.InitializeAsync();
            //CaptureElement.Source = _mediaCapture;
            //await _mediaCapture.StartPreviewAsync();
        }
    }
}
