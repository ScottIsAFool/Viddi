using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Cimbalino.Toolkit.Services;

namespace Viddy.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class VideoPlayerView
    {
        public VideoPlayerView()
        {
            InitializeComponent();
        }

        private void MediaPlayer_OnMediaFailed(object sender, Windows.UI.Xaml.ExceptionRoutedEventArgs e)
        {

        }

        private void MediaPlayer_OnIsFullScreenChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            MediaPlayer.IsFullWindow = MediaPlayer.IsFullScreen;
        }

        protected override void OnBackKeyPressed(object sender, NavigationServiceBackKeyPressedEventArgs e)
        {
            e.Behavior = MediaPlayer.IsFullWindow ? NavigationServiceBackKeyPressedBehavior.DoNothing : NavigationServiceBackKeyPressedBehavior.GoBack;

            MediaPlayer.IsFullScreen = false;
            MediaPlayer.IsFullWindow = false;

            base.OnBackKeyPressed(sender, e);
        }
    }
}
