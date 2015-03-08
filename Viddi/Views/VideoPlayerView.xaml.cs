using Windows.UI.Xaml;
using Cimbalino.Toolkit.Services;
using Microsoft.PlayerFramework;

namespace Viddi.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class VideoPlayerView
    {
        private MediaPlayer _mediaPlayer;

        public VideoPlayerView()
        {
            InitializeComponent();
        }

        private void MediaPlayer_OnMediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            Logger.Error("MediaPlayer Media Failed: " + e.ErrorMessage);
        }

        private void MediaPlayer_OnIsFullScreenChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            var mediaPlayer = sender as MediaPlayer;
            if (mediaPlayer != null)
            {
                mediaPlayer.IsFullWindow = mediaPlayer.IsFullScreen;
            }
        }

        protected override void OnBackKeyPressed(object sender, NavigationServiceBackKeyPressedEventArgs e)
        {
            if (_mediaPlayer != null)
            {
                e.Behavior = _mediaPlayer.IsFullWindow ? NavigationServiceBackKeyPressedBehavior.DoNothing : NavigationServiceBackKeyPressedBehavior.GoBack;

                _mediaPlayer.IsFullScreen = false;
                _mediaPlayer.IsFullWindow = false;
            }

            base.OnBackKeyPressed(sender, e);
        }

        private async void PinButton_OnClick(object sender, RoutedEventArgs e)
        {
            await SaveTileImage(MediumTile);
        }

        private void MediaPlayer_OnLoaded(object sender, RoutedEventArgs e)
        {
            var mediaPlayer = sender as MediaPlayer;
            if (mediaPlayer != null)
            {
                _mediaPlayer = mediaPlayer;
            }
        }
    }
}
