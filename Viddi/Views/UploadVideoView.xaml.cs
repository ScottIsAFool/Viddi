using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Cimbalino.Toolkit.Services;
using Viddi.ViewModel;

namespace Viddi.Views
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

        protected override void OnBackKeyPressed(object sender, NavigationServiceBackKeyPressedEventArgs e)
        {
            var vm = DataContext as UploadVideoViewModel;
            if (vm != null)
            {
                vm.EditVideo.TryGoingBack();
                e.Behavior = NavigationServiceBackKeyPressedBehavior.DoNothing;
            }

            base.OnBackKeyPressed(sender, e);
        }
    }
}
