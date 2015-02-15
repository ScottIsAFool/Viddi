using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace Viddy.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UploadVideoView 
    {
        public UploadVideoView()
        {
            InitializeComponent();
        }

        protected override ApplicationViewBoundsMode Mode
        {
            get { return ApplicationViewBoundsMode.UseCoreWindow; }
        }

        private void MediaPlayer_OnMediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            var s = "";
        }
    }
}
