using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace Viddy.Views.Account.Manage
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ManageMyAppsView
    {
        public ManageMyAppsView()
        {
            InitializeComponent();
        }

        private void MenuButton_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            var button = sender as FrameworkElement;
            FlyoutBase.ShowAttachedFlyout(button);
        }
    }
}
