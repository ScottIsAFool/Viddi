using Windows.UI.Xaml;

namespace Viddi.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ChannelView 
    {
        public ChannelView()
        {
            InitializeComponent();
        }

        private async void PinButton_OnClick(object sender, RoutedEventArgs e)
        {
            await SaveTileImage(MediumTile, WideTile);
        }
    }
}
