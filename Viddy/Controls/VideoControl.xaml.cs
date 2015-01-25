using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Viddy.ViewModel;

namespace Viddy.Controls
{
    public sealed partial class VideoControl
    {
        public VideoControl()
        {
            InitializeComponent();
        }

        private void ContainerGrid_OnHolding(object sender, HoldingRoutedEventArgs e)
        {
            var vm = DataContext as VideoItemViewModel;
            if (vm == null || !vm.CanDelete)
            {
                return;
            }

            var grid = sender as Grid;
            FlyoutBase.ShowAttachedFlyout(grid);
        }
    }
}
