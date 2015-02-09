using Windows.UI.Xaml;

namespace Viddy.Views.Account
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProfileView
    {
        public ProfileView()
        {
            InitializeComponent();
        }

        private async void PinButton_OnClick(object sender, RoutedEventArgs e)
        {
            await SaveTileImage(MediumTile, WideTile);
        }
    }
}
