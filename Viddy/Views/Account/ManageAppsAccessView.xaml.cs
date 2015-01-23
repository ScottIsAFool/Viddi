using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace Viddy.Views.Account
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ManageAppsAccessView
    {
        public ManageAppsAccessView()
        {
            InitializeComponent();
        }

        private void RevokeButton_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            var button = sender as Button;
            FlyoutBase.ShowAttachedFlyout(button);
        }
    }
}
